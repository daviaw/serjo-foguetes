using System.Collections;
using UnityEngine;

/// <summary>
/// Controla o jogador no jogo Meteor Storm:
/// - Movimento suave em 8 direcoes (WASD e Setas)
/// - Inclinacao visual organica (banking) ao se mover para os lados
/// - Delimitacao rigida aos limites da tela com screenPadding
/// - Sistema de vidas (3 vidas iniciais), invulnerabilidade temporaria e efeito de piscar
/// - Suporte a Escudo de energia e Vida Extra
/// - Emissao de particulas no motor
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movimentacao")]
    [Tooltip("Velocidade de movimento da nave.")]
    [SerializeField] private float moveSpeed = 8.5f;

    [Tooltip("Distancia de seguranca em relacao as bordas da tela.")]
    [SerializeField] private float screenPadding = 0.6f;

    [Header("Inclinacao Visual (Tilt)")]
    [Tooltip("Angulo maximo de inclinacao nas curvas laterais.")]
    [SerializeField] private float maxTiltAngle = 18f;

    [Tooltip("Velocidade de transicao da inclinacao.")]
    [SerializeField] private float tiltSmoothSpeed = 10f;

    [Header("Sistema de Vidas")]
    [Tooltip("Quantidade inicial de vidas.")]
    [SerializeField] private int maxLives = 3;

    [Tooltip("Tempo de invulnerabilidade apos sofrer dano.")]
    [SerializeField] private float invulnerabilityDuration = 2.0f;

    [Header("Efeitos e Componentes")]
    [SerializeField] private ParticleSystem engineTrail;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject shieldVisual;

    private int currentLives;
    private bool isInvulnerable = false;
    private bool isDead = false;
    private bool hasShield = false;
    private float shieldTimer = 0f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D playerCollider;

    private float minX, maxX, minY, maxY;

    public int CurrentLives => currentLives;
    public bool IsDead => isDead;
    public bool HasShield => hasShield;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Start()
    {
        currentLives = maxLives;
        isDead = false;
        isInvulnerable = false;

        CalculateScreenBounds();

        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateLives(currentLives);
        }

        if (engineTrail != null && !engineTrail.isPlaying)
        {
            engineTrail.Play();
        }
    }

    private void Update()
    {
        if (isDead) return;

        UpdateShieldTimer();
        HandleMovement();
        UpdateTilt();
    }

    private void CalculateScreenBounds()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            float vertExtent = cam.orthographicSize;
            float horzExtent = vertExtent * cam.aspect;

            minX = -horzExtent + screenPadding;
            maxX = horzExtent - screenPadding;
            minY = -vertExtent + screenPadding;
            maxY = vertExtent - screenPadding;
        }
        else
        {
            minX = -8.5f + screenPadding;
            maxX = 8.5f - screenPadding;
            minY = -4.5f + screenPadding;
            maxY = 4.5f - screenPadding;
        }
    }

    private void HandleMovement()
    {
        float moveX = 0f;
        float moveY = 0f;

        // Suporte a WASD e Setas
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveX -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveX += 1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveY += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveY -= 1f;

        Vector2 movement = new Vector2(moveX, moveY);
        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        Vector3 newPos = transform.position + (Vector3)(movement * (moveSpeed * Time.deltaTime));

        // Limita a nave aos limites visiveis da tela
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        transform.position = newPos;
    }

    private void UpdateTilt()
    {
        float moveX = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveX -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveX += 1f;

        // Inclinacao: esquerda inclina para esquerda (+Z), direita para direita (-Z)
        float targetZAngle = -moveX * maxTiltAngle;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZAngle);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, tiltSmoothSpeed * Time.deltaTime);
    }

    private void UpdateShieldTimer()
    {
        if (hasShield)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0f)
            {
                DeactivateShield();
            }
        }
    }

    public void TakeDamage()
    {
        if (isDead || isInvulnerable) return;

        // Se estiver protegido por escudo, consome o escudo sem perder vidas
        if (hasShield)
        {
            DeactivateShield();
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayShieldHit();
            }
            StartCoroutine(InvulnerabilityRoutine(1.0f));
            return;
        }

        currentLives--;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateLives(currentLives);
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.35f, 0.4f);
        }

        if (currentLives <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine(invulnerabilityDuration));
        }
    }

    private IEnumerator InvulnerabilityRoutine(float duration)
    {
        isInvulnerable = true;
        float elapsed = 0f;
        float flashInterval = 0.12f;

        while (elapsed < duration)
        {
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = (c.a > 0.5f) ? 0.2f : 1f;
                spriteRenderer.color = c;
            }

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }

        isInvulnerable = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (engineTrail != null)
        {
            engineTrail.Stop();
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void AddLife()
    {
        currentLives++;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateLives(currentLives);
        }
    }

    public void ActivateShield(float duration)
    {
        hasShield = true;
        shieldTimer = duration;
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }
    }

    private void DeactivateShield()
    {
        hasShield = false;
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }
}
