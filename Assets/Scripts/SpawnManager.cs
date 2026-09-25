using System.Collections;
using UnityEngine;

/// <summary>
/// Responsavel pelo spawn e dificuldade infinita no jogo Meteor Storm:
/// - Spawn continuo na borda superior da tela em posicoes horizontais aleatorias.
/// - Selecao aleatoria e balanceada de tipos de meteoros (Pequeno, Medio, Grande).
/// - Dificuldade progressiva infinita:
///     * Intervalo de spawn diminui continuamente (mais meteoros por segundo).
///     * Velocidade de queda aumenta continuamente.
///     * Frequencia de meteoros grandes cresce progressivamente.
///     * Sem teto maximo.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs de Meteoros")]
    [SerializeField] private GameObject meteorSmallPrefab;
    [SerializeField] private GameObject meteorMediumPrefab;
    [SerializeField] private GameObject meteorLargePrefab;

    [Header("Configuracao de Spawn")]
    [Tooltip("Intervalo inicial entre spawns de meteoros em segundos.")]
    [SerializeField] private float spawnRate = 1.6f;

    [Tooltip("Largura horizontal da area de spawn no topo.")]
    [SerializeField] private float spawnAreaWidth = 15f;

    [Tooltip("Altura Y fora da tela superior onde os meteoros surgem.")]
    [SerializeField] private float spawnPositionY = 6.8f;

    [Header("Dificuldade Infinita")]
    [Tooltip("Fator de aceleracao da dificuldade com o tempo.")]
    [SerializeField] private float difficultyMultiplier = 0.035f;

    private float elapsedTime = 0f;
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;

    private void Start()
    {
        StartSpawning();
    }

    private void Update()
    {
        if (!isSpawning) return;
        elapsedTime += Time.deltaTime;
    }

    public void StartSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        isSpawning = true;
        elapsedTime = 0f;
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(0.6f);

        while (isSpawning)
        {
            SpawnMeteor();

            // Intervalo escala infinitamente (fica cada vez menor conforme o tempo passa)
            // Ex: 1.6s -> 1.0s -> 0.6s -> 0.35s -> 0.2s...
            float currentSpawnInterval = spawnRate / (1f + (elapsedTime * difficultyMultiplier * 0.7f));
            currentSpawnInterval = Mathf.Max(0.12f, currentSpawnInterval);

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    private void SpawnMeteor()
    {
        // Multiplicador infinito de velocidade
        float speedMultiplier = 1f + (elapsedTime * difficultyMultiplier * 0.45f);

        // Posicao horizontal aleatoria
        float randomX = Random.Range(-spawnAreaWidth * 0.5f, spawnAreaWidth * 0.5f);
        Vector3 spawnPos = new Vector3(randomX, spawnPositionY, 0f);

        // Probabilidades de tamanho baseadas no tempo de sobrevivencia
        // Cedo: 70% pequeno, 25% medio, 5% grande
        // Mais tarde: 30% pequeno, 40% medio, 30% grande
        float largeProb = Mathf.Clamp(0.05f + (elapsedTime * 0.003f), 0.05f, 0.45f);
        float mediumProb = Mathf.Clamp(0.25f + (elapsedTime * 0.002f), 0.25f, 0.45f);

        float roll = Random.value;
        GameObject selectedPrefab;
        MeteorSize chosenSize;

        if (roll < largeProb && meteorLargePrefab != null)
        {
            selectedPrefab = meteorLargePrefab;
            chosenSize = MeteorSize.Large;
        }
        else if (roll < (largeProb + mediumProb) && meteorMediumPrefab != null)
        {
            selectedPrefab = meteorMediumPrefab;
            chosenSize = MeteorSize.Medium;
        }
        else
        {
            selectedPrefab = meteorSmallPrefab ?? meteorMediumPrefab ?? meteorLargePrefab;
            chosenSize = MeteorSize.Small;
        }

        if (selectedPrefab != null)
        {
            GameObject meteorObj = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
            Meteor meteor = meteorObj.GetComponent<Meteor>();
            if (meteor != null)
            {
                meteor.Initialize(chosenSize, speedMultiplier);
            }
        }
    }
}
