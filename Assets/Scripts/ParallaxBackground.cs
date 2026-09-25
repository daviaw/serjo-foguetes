using UnityEngine;

/// <summary>
/// Cria um fundo espacial infinito com multiplas camadas e efeito Parallax:
/// - Cada camada contem dois sprites que se repetem perfeitamente no eixo vertical (rolagem infinita para baixo).
/// - Suporta velocidades diferentes para criar sensacao real de profundidade (nebulosas distantes, estrelas intermediarias e particulas).
/// - Responde sutilmente a movimentacao horizontal do jogador para maior imersao.
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public string layerName = "Layer";
        public Transform spriteTransformA;
        public Transform spriteTransformB;
        public float scrollSpeed = 2f;
        public float layerHeight = 16f;
        [Range(0f, 0.5f)] public float horizontalParallaxFactor = 0.05f;
    }

    [Header("Camadas de Parallax")]
    [Tooltip("Configuracao de cada camada visual do espaco.")]
    [SerializeField] private ParallaxLayer[] layers;

    [Header("Referencia Opcional")]
    [SerializeField] private Transform playerTransform;

    private float previousPlayerX = 0f;

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        if (playerTransform != null)
        {
            previousPlayerX = playerTransform.position.x;
        }

        // Posiciona a copia B de cada camada exatamente acima da copia A caso necessario
        if (layers != null)
        {
            foreach (var layer in layers)
            {
                if (layer.spriteTransformA != null && layer.spriteTransformB != null)
                {
                    Vector3 posB = layer.spriteTransformA.position;
                    posB.y += layer.layerHeight;
                    layer.spriteTransformB.position = posB;
                }
            }
        }
    }

    private void Update()
    {
        float playerDeltaX = 0f;
        if (playerTransform != null)
        {
            playerDeltaX = playerTransform.position.x - previousPlayerX;
            previousPlayerX = playerTransform.position.x;
        }

        if (layers == null) return;

        foreach (var layer in layers)
        {
            if (layer.spriteTransformA == null || layer.spriteTransformB == null) continue;

            float verticalMovement = layer.scrollSpeed * Time.deltaTime;
            float horizontalMovement = -playerDeltaX * layer.horizontalParallaxFactor;

            // Move ambos os sprites da camada
            layer.spriteTransformA.position += new Vector3(horizontalMovement, -verticalMovement, 0f);
            layer.spriteTransformB.position += new Vector3(horizontalMovement, -verticalMovement, 0f);

            // Verifica reposicionamento ciclico
            CheckAndWrap(layer.spriteTransformA, layer.spriteTransformB, layer.layerHeight);
            CheckAndWrap(layer.spriteTransformB, layer.spriteTransformA, layer.layerHeight);
        }
    }

    private void CheckAndWrap(Transform checking, Transform other, float height)
    {
        // Se o sprite saiu completamente pela borda inferior da tela
        if (checking.position.y <= -height)
        {
            Vector3 targetPos = checking.position;
            targetPos.y = other.position.y + height - 0.05f; // Pequeno ajuste para evitar costura visivel
            checking.position = targetPos;
        }
    }
}
