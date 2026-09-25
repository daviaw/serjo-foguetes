using UnityEngine;

/// <summary>
/// Controla o movimento do jogador (WASD / setas) com limites de tela.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f; // velocidade do jogador
    [SerializeField] private Vector2 minBounds;    // limites inferiores da câmera
    [SerializeField] private Vector2 maxBounds;    // limites superiores da câmera

    private Rigidbody2D rb;
    private Vector2 input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Captura entrada do jogador (WASD e setas)
        float h = Input.GetAxisRaw("Horizontal"); // -1, 0, 1
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector2(h, v).normalized;
    }

    private void FixedUpdate()
    {
        // Move o jogador usando Rigidbody2D para manter consistência com física
        Vector2 newPos = rb.position + input * moveSpeed * Time.fixedDeltaTime;

        // Aplica limites para não sair da tela
        newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
        newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);

        rb.MovePosition(newPos);
    }

    // Método público para reduzir vida ou reagir a colisões (chamado por GameManager)
    public void OnHit()
    {
        Debug.Log("Player hit!");
    }
}
