using System.Collections;
using UnityEngine;

public enum MeteorSize
{
    Small,
    Medium,
    Large
}

/// <summary>
/// Controla o comportamento de cada meteoro:
/// - Queda contínua com velocidade e rotação aleatórias.
/// - Vida proporcional ao tamanho (Pequeno, Médio, Grande).
/// - Pontuação por destruição (50, 100, 200).
/// - Chance de soltar Power-ups ao ser destruído.
/// - Efeito de Camera Shake em explosões de meteoros maiores.
/// </summary>
public class Meteor : MonoBehaviour
{
    [Header("Configurações do Meteoro")]
    [SerializeField] private MeteorSize meteorSize = MeteorSize.Medium;
    [SerializeField] private int health = 3;
    [SerializeField] private float fallSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private int scoreValue = 100;
    [SerializeField] private float despawnY = -7f;

    [Header("Chance de Power-up")]
    [Range(0f, 1f)] [SerializeField] private float dropChance = 0.22f;
    [SerializeField] private GameObject powerUpPrefab;

    [Header("Efeitos Visuais")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float rotationDirection = 1f;
    private bool isDead = false;
    private Color originalColor = Color.white;
    private Coroutine flashCoroutine;

    public MeteorSize Size => meteorSize;
    public int ScoreValue => scoreValue;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Variação de rotação e velocidade
        rotationDirection = Random.value > 0.5f ? 1f : -1f;
        rotationSpeed *= Random.Range(0.8f, 1.4f);
    }

    private void Update()
    {
        if (isDead) return;

        // Queda vertical
        transform.Translate(Vector3.down * (fallSpeed * Time.deltaTime), Space.World);

        // Rotação contínua
        transform.Rotate(0f, 0f, rotationSpeed * rotationDirection * Time.deltaTime);

        // Autodestruição ao sair da tela
        if (transform.position.y < despawnY)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Permite ao SpawnManager inicializar parâmetros com base na dificuldade atual.
    /// </summary>
    public void Initialize(MeteorSize size, float speedMultiplier = 1f)
    {
        meteorSize = size;

        switch (size)
        {
            case MeteorSize.Small:
                health = 1;
                fallSpeed = Random.Range(4.5f, 6.0f) * speedMultiplier;
                scoreValue = 50;
                break;
            case MeteorSize.Medium:
                health = 3;
                fallSpeed = Random.Range(3.0f, 4.5f) * speedMultiplier;
                scoreValue = 100;
                break;
            case MeteorSize.Large:
                health = 6;
                fallSpeed = Random.Range(2.0f, 3.2f) * speedMultiplier;
                scoreValue = 200;
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(DamageFlashRoutine());

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.4f, 0.4f, 1f);
            yield return new WaitForSeconds(0.08f);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Feedback de Áudio
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExplosion(meteorSize == MeteorSize.Large);
        }

        // Camera Shake em explosões maiores
        if (meteorSize == MeteorSize.Large && CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.35f, 0.45f);
        }
        else if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.15f, 0.15f);
        }

        // Partículas de explosão
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Pontuação
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // Chance de soltar Power-up
        TryDropPowerUp();

        Destroy(gameObject);
    }

    private void TryDropPowerUp()
    {
        if (powerUpPrefab == null) return;

        if (Random.value <= dropChance)
        {
            GameObject spawnedPowerUp = Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
            PowerUp pu = spawnedPowerUp.GetComponent<PowerUp>();
            if (pu != null)
            {
                // Escolhe aleatoriamente um dos tipos de bônus
                PowerUpType[] types = (PowerUpType[])System.Enum.GetValues(typeof(PowerUpType));
                PowerUpType chosenType = types[Random.Range(0, types.Length)];
                pu.Setup(chosenType, 10f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Se colidir diretamente com o jogador
        if (collision.CompareTag("player") || collision.GetComponent<PlayerController>() != null)
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage();
            }
            Die();
        }
    }
}
