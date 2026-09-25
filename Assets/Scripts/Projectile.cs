using UnityEngine;

/// <summary>
/// Controla o comportamento de cada projetil / tiro de laser:
/// - Movimento constante para cima na velocidade configurada.
/// - Auto destruicao ao sair da tela pelo topo.
/// - Deteccao de colisao com meteoros, aplicando dano e instanciando faiscas de impacto.
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Configuracoes do Projetil")]
    [Tooltip("Velocidade de deslocamento vertical.")]
    [SerializeField] private float projectileSpeed = 14f;

    [Tooltip("Quantidade de dano infligida ao meteoro.")]
    [SerializeField] private int projectileDamage = 1;

    [Tooltip("Limite superior em Y para autodestruicao.")]
    [SerializeField] private float despawnY = 6.5f;

    [Header("Efeitos")]
    [Tooltip("Prefab do efeito de faiscas de impacto.")]
    [SerializeField] private GameObject impactPrefab;

    private void Update()
    {
        // Move o projetil para cima
        transform.Translate(Vector3.up * (projectileSpeed * Time.deltaTime), Space.World);

        // Destroi se passar do limite superior da tela
        if (transform.position.y > despawnY)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica colisao com meteoro
        Meteor meteor = collision.GetComponent<Meteor>();
        if (meteor != null)
        {
            meteor.TakeDamage(projectileDamage);

            // Instancia particulas de impacto
            if (impactPrefab != null)
            {
                Instantiate(impactPrefab, transform.position, Quaternion.identity);
            }

            // Som de impacto
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayImpact();
            }

            Destroy(gameObject);
        }
    }

    public void SetDamage(int damage)
    {
        projectileDamage = damage;
    }

    public void SetSpeed(float speed)
    {
        projectileSpeed = speed;
    }
}
