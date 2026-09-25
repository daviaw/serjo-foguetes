using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gerencia estado do jogo: score, vidas, Game Over.
/// Singleton simples para acesso global.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int startingLives = 3;
    private int lives;
    private int score;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        lives = startingLives;
        score = 0;
        UIManager.Instance.UpdateScore(score);
        UIManager.Instance.UpdateLives(lives);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UIManager.Instance.UpdateScore(score);
    }

    public void PlayerHit()
    {
        lives--;
        UIManager.Instance.UpdateLives(lives);

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            // opcional: reiniciar posição do player
            // GameObject.FindWithTag("Player").transform.position = new Vector3(-6, 0, 0);
        }
    }

    private void GameOver()
    {
        // Mostrar painel Game Over
        // Substitua a chamada para ShowGameOver() por um método existente ou implemente ShowGameOver em UIManager.
        // Exemplo temporário: apenas exiba um log ou utilize um método já existente.
        Debug.Log("Game Over!");

        // Pausar o jogo (opcional)
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
