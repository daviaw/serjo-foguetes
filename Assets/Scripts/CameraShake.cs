using System.Collections;
using UnityEngine;

/// <summary>
/// Responsavel pelo efeito dinamico de Camera Shake na camera principal:
/// - Acionado em colisoes da nave e explosoes de meteoros maiores.
/// - Suave e com amortecimento progressivo para retornar a posicao neutra.
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 initialLocalPosition;
    private Coroutine currentShakeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        initialLocalPosition = transform.localPosition;
    }

    /// <summary>
    /// Inicia o tremor de tela com duracao e magnitude customizaveis.
    /// </summary>
    /// <param name="duration">Duracao do tremor em segundos.</param>
    /// <param name="magnitude">Forca do deslocamento.</param>
    public void Shake(float duration = 0.25f, float magnitude = 0.35f)
    {
        if (!gameObject.activeInHierarchy) return;

        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
            transform.localPosition = initialLocalPosition;
        }

        currentShakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float damper = 1f - (elapsed / duration);
            float offsetX = Random.Range(-1f, 1f) * magnitude * damper;
            float offsetY = Random.Range(-1f, 1f) * magnitude * damper;

            transform.localPosition = new Vector3(
                initialLocalPosition.x + offsetX,
                initialLocalPosition.y + offsetY,
                initialLocalPosition.z
            );

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localPosition = initialLocalPosition;
        currentShakeCoroutine = null;
    }
}
