using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla o menu principal do Meteor Storm:
/// - Exibe o recorde atual do jogador.
/// - Inicia a partida ao clicar no botao.
/// </summary>
public class StartButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Start()
    {
        if (highScoreText != null)
        {
            int best = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = $"RECORDE: {best:D4}";
        }
    }

    public void IniciarJogo()
    {
        SceneManager.LoadScene("jogo");
    }
}
