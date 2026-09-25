using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Singleton central do jogo Meteor Storm:
/// - Gerenciamento de estado (Jogando vs Game Over).
/// - Pontuacao por destruicao de meteoros (50/100/200) e por sobrevivencia (+1 pt/s).
/// - Salvamento e carregamento de High Score via PlayerPrefs.
/// - Atualizacao da interface moderna do HUD e do painel de Game Over em tempo real.
/// - Reinicio completo da partida pelo botao Restart.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("HUD - TextMeshPro")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI survivalTimeText;

    [Header("Painel de Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI finalSurvivalTimeText;
    [SerializeField] private Button restartButton;

    [Header("Referencias")]
    [SerializeField] private SpawnManager spawnManager;

    private int score = 0;
    private int highScore = 0;
    private int currentLives = 3;
    private float survivalTimer = 0f;
    private float secondTimer = 0f;
    private bool isGameOver = false;

    public bool IsGameOver => isGameOver;
    public int CurrentScore => score;
    public int HighScore => highScore;
    public float SurvivalTime => survivalTimer;

    private const string HIGH_SCORE_KEY = "HighScore";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        // Carrega recorde
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);

        score = 0;
        survivalTimer = 0f;
        secondTimer = 0f;
        isGameOver = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        if (spawnManager == null)
        {
            spawnManager = FindFirstObjectByType<SpawnManager>();
        }

        UpdateHUD();
    }

    private void Update()
    {
        if (isGameOver) return;

        survivalTimer += Time.deltaTime;
        secondTimer += Time.deltaTime;

        // +1 ponto por segundo sobrevivido
        if (secondTimer >= 1.0f)
        {
            secondTimer -= 1.0f;
            score += 1;

            if (score > highScore)
            {
                highScore = score;
            }
        }

        UpdateHUD();
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;
        if (score > highScore)
        {
            highScore = score;
        }

        UpdateHUD();
    }

    public void UpdateLives(int lives)
    {
        currentLives = Mathf.Max(0, lives);
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {score:D4}";
        }

        if (highScoreText != null)
        {
            highScoreText.text = $"BEST: {highScore:D4}";
        }

        if (livesText != null)
        {
            livesText.text = $"LIVES: {currentLives}";
        }

        if (survivalTimeText != null)
        {
            int minutes = Mathf.FloorToInt(survivalTimer / 60f);
            int seconds = Mathf.FloorToInt(survivalTimer % 60f);
            survivalTimeText.text = $"TIME: {minutes:00}:{seconds:00}";
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (spawnManager != null)
        {
            spawnManager.StopSpawning();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOver();
        }

        // Salva novo recorde
        if (score >= highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }

        // Configura e ativa painel de Game Over
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Score: {score:D4}";
        }

        if (bestScoreText != null)
        {
            bestScoreText.text = $"Best: {highScore:D4}";
        }

        if (finalSurvivalTimeText != null)
        {
            int minutes = Mathf.FloorToInt(survivalTimer / 60f);
            int seconds = Mathf.FloorToInt(survivalTimer % 60f);
            finalSurvivalTimeText.text = $"Survival Time: {minutes:00}:{seconds:00}";
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
