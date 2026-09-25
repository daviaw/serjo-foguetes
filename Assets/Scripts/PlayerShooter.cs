using UnityEngine;

/// <summary>
/// Controla o sistema de disparos do jogador:
/// - Disparo continuo ao segurar Espaco ou Clique Esquerdo do mouse.
/// - Cadencia de tiro configuravel com cooldown.
/// - Suporte dinamico aos Power-ups (Tiro Duplo, Tiro Triplo, Tiro Rapido).
/// </summary>
public class PlayerShooter : MonoBehaviour
{
    [Header("Configuracao de Disparo")]
    [Tooltip("Intervalo minimo em segundos entre disparos.")]
    [SerializeField] private float fireRate = 0.22f;

    [Tooltip("Velocidade dos projeteis lancados.")]
    [SerializeField] private float projectileSpeed = 15f;

    [Tooltip("Dano de cada projetil.")]
    [SerializeField] private int projectileDamage = 1;

    [Header("Prefabs e Pontos de Disparo")]
    [Tooltip("Prefab do projetil / laser.")]
    [SerializeField] private GameObject projectilePrefab;

    [Tooltip("Ponto central de onde os tiros partem.")]
    [SerializeField] private Transform firePoint;

    private float nextFireTime = 0f;
    private PowerUpType currentWeaponMode = PowerUpType.DoubleShot; // Default single shot logic when no weapon buff
    private bool hasWeaponBuff = false;
    private float weaponBuffTimer = 0f;
    private bool hasRapidFire = false;
    private float rapidFireTimer = 0f;

    private void Start()
    {
        if (firePoint == null)
        {
            Transform gunChild = transform.Find("playergun");
            if (gunChild != null)
            {
                firePoint = gunChild;
            }
            else
            {
                firePoint = transform;
            }
        }
    }

    private void Update()
    {
        // Se a partida encerrou, nao atira
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            return;
        }

        UpdatePowerUpTimers();
        HandleShootingInput();
    }

    private void UpdatePowerUpTimers()
    {
        if (hasWeaponBuff)
        {
            weaponBuffTimer -= Time.deltaTime;
            if (weaponBuffTimer <= 0f)
            {
                hasWeaponBuff = false;
            }
        }

        if (hasRapidFire)
        {
            rapidFireTimer -= Time.deltaTime;
            if (rapidFireTimer <= 0f)
            {
                hasRapidFire = false;
            }
        }
    }

    private void HandleShootingInput()
    {
        // Disparo continuo: segura espaco ou botao esquerdo
        bool isHoldingFire = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);

        if (isHoldingFire && Time.time >= nextFireTime)
        {
            float currentRate = hasRapidFire ? (fireRate * 0.5f) : fireRate;
            nextFireTime = Time.time + currentRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Vector3 spawnPos = firePoint.position;

        if (hasWeaponBuff && currentWeaponMode == PowerUpType.TripleShot)
        {
            // Tiro Triplo: Centro + Diagonal Esquerda (-15) + Diagonal Direita (+15)
            SpawnProjectile(spawnPos, Quaternion.identity);
            SpawnProjectile(spawnPos + new Vector3(-0.25f, -0.1f, 0f), Quaternion.Euler(0f, 0f, 15f));
            SpawnProjectile(spawnPos + new Vector3(0.25f, -0.1f, 0f), Quaternion.Euler(0f, 0f, -15f));
        }
        else if (hasWeaponBuff && currentWeaponMode == PowerUpType.DoubleShot)
        {
            // Tiro Duplo: Dois tiros paralelos
            SpawnProjectile(spawnPos + new Vector3(-0.3f, 0f, 0f), Quaternion.identity);
            SpawnProjectile(spawnPos + new Vector3(0.3f, 0f, 0f), Quaternion.identity);
        }
        else
        {
            // Tiro Padrao: Um projetil central
            SpawnProjectile(spawnPos, Quaternion.identity);
        }

        // Toca o som do disparo
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayLaser();
        }
    }

    private void SpawnProjectile(Vector3 position, Quaternion rotation)
    {
        GameObject projObj = Instantiate(projectilePrefab, position, rotation);
        Projectile proj = projObj.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.SetSpeed(projectileSpeed);
            proj.SetDamage(projectileDamage);
        }
    }

    /// <summary>
    /// Ativa bonus de tiro obtido via Power-up.
    /// </summary>
    public void ActivatePowerUp(PowerUpType type, float duration)
    {
        if (type == PowerUpType.RapidFire)
        {
            hasRapidFire = true;
            rapidFireTimer = duration;
        }
        else if (type == PowerUpType.DoubleShot || type == PowerUpType.TripleShot)
        {
            hasWeaponBuff = true;
            currentWeaponMode = type;
            weaponBuffTimer = duration;
        }
    }
}
