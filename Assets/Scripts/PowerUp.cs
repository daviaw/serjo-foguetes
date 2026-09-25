using UnityEngine;

/// <summary>
/// Tipos de Power-ups disponiveis no jogo
/// </summary>
public enum PowerUpType
{
    DoubleShot,
    TripleShot,
    Shield,
    ExtraLife,
    RapidFire
}

/// <summary>
/// Controla o comportamento de cada item de Power-up:
/// - Desce suavemente pela tela com oscilacao visual.
/// - Aplica o efeito correspondente ao jogador quando coletado.
/// - Suporta cores e efeitos tematicos para cada tipo de bonus.
/// </summary>
public class PowerUp : MonoBehaviour
{
    [Header("Configuracao do Power-up")]
    [SerializeField] private PowerUpType type = PowerUpType.DoubleShot;
    [SerializeField] private float duration = 10f;
    [SerializeField] private float fallSpeed = 2f;
    [SerializeField] private float despawnY = -6.5f;

    [Header("Efeitos Visuais")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject pickupEffectPrefab;

    private float bobTimer = 0f;

    public PowerUpType Type => type;
    public float Duration => duration;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        ApplyVisualTheme();
    }

    private void Update()
    {
        // Deslocamento para baixo
        transform.Translate(Vector3.down * (fallSpeed * Time.deltaTime), Space.World);

        // Animacao suave de flutuacao / pulso
        bobTimer += Time.deltaTime * 5f;
        float pulse = 1f + 0.12f * Mathf.Sin(bobTimer);
        transform.localScale = new Vector3(pulse, pulse, 1f);

        // Destroi se sair da tela
        if (transform.position.y < despawnY)
        {
            Destroy(gameObject);
        }
    }

    public void Setup(PowerUpType newType, float newDuration = 10f)
    {
        type = newType;
        duration = newDuration;
        ApplyVisualTheme();
    }

    private void ApplyVisualTheme()
    {
        if (spriteRenderer == null) return;

        switch (type)
        {
            case PowerUpType.DoubleShot:
                spriteRenderer.color = new Color(0.2f, 0.9f, 1f, 1f); // Ciano
                break;
            case PowerUpType.TripleShot:
                spriteRenderer.color = new Color(0.85f, 0.3f, 1f, 1f); // Roxo / Magenta
                break;
            case PowerUpType.Shield:
                spriteRenderer.color = new Color(0.3f, 0.6f, 1f, 1f); // Azul Escudo
                break;
            case PowerUpType.ExtraLife:
                spriteRenderer.color = new Color(0.3f, 1f, 0.4f, 1f); // Verde Vida
                break;
            case PowerUpType.RapidFire:
                spriteRenderer.color = new Color(1f, 0.85f, 0.1f, 1f); // Dourado
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("player") && collision.GetComponent<PlayerController>() == null)
        {
            return;
        }

        PlayerController player = collision.GetComponent<PlayerController>();
        PlayerShooter shooter = collision.GetComponent<PlayerShooter>();

        bool collected = false;

        switch (type)
        {
            case PowerUpType.ExtraLife:
                if (player != null)
                {
                    player.AddLife();
                    collected = true;
                }
                break;

            case PowerUpType.Shield:
                if (player != null)
                {
                    player.ActivateShield(duration);
                    collected = true;
                }
                break;

            case PowerUpType.DoubleShot:
            case PowerUpType.TripleShot:
            case PowerUpType.RapidFire:
                if (shooter != null)
                {
                    shooter.ActivatePowerUp(type, duration);
                    collected = true;
                }
                break;
        }

        if (collected)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPowerUp();
            }

            if (pickupEffectPrefab != null)
            {
                Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
