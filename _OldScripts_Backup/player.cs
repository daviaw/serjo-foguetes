using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    // 🔫 Campos do tiro
    [SerializeField] private GameObject bulletPrefab;   // Prefab do Bullet
    [SerializeField] private Transform gunPoint;        // Ponto de origem do tiro

    private void Update()
    {
        // Movimento do Player
        float moveY = Input.GetAxis("Vertical");
        transform.Translate(Vector2.up * moveY * speed * Time.deltaTime);

        // Disparo do Player
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Cria o Bullet na posição do gunPoint
        Instantiate(bulletPrefab, gunPoint.position, gunPoint.rotation);
    }
}
