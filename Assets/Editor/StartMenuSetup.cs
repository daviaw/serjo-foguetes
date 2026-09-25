using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuSetup
{
    [MenuItem("Meteor Storm/Setup Start Menu")]
    public static void Setup()
    {
        string scenePath = "Assets/Scenes/tela inicio.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        // Atualiza o texto do botao existente
        Button btn = Object.FindFirstObjectByType<Button>();
        if (btn != null)
        {
            TextMeshProUGUI btnTmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (btnTmp != null)
            {
                btnTmp.text = "INICIAR JOGO";
                btnTmp.fontSize = 28;
                btnTmp.fontStyle = FontStyles.Bold;
                btnTmp.color = Color.white;
                btnTmp.alignment = TextAlignmentOptions.Center;
                EditorUtility.SetDirty(btnTmp);
            }

            RectTransform btnRt = btn.GetComponent<RectTransform>();
            if (btnRt != null)
            {
                btnRt.anchoredPosition = new Vector2(0f, -140f);
                btnRt.sizeDelta = new Vector2(280f, 65f);
            }
        }

        // Procura ou cria Titulo
        Transform titleTransform = canvas.transform.Find("TitleText");
        TextMeshProUGUI titleTmp;
        if (titleTransform == null)
        {
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(canvas.transform, false);
            titleTmp = titleObj.AddComponent<TextMeshProUGUI>();
        }
        else
        {
            titleTmp = titleTransform.GetComponent<TextMeshProUGUI>();
        }

        titleTmp.text = "METEOR STORM";
        titleTmp.fontSize = 76;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.color = new Color(1f, 0.4f, 0.15f, 1f);
        titleTmp.alignment = TextAlignmentOptions.Center;
        RectTransform titleRt = titleTmp.rectTransform;
        titleRt.anchorMin = new Vector2(0.5f, 0.5f);
        titleRt.anchorMax = new Vector2(0.5f, 0.5f);
        titleRt.anchoredPosition = new Vector2(0f, 220f);
        titleRt.sizeDelta = new Vector2(800f, 90f);
        EditorUtility.SetDirty(titleTmp);

        // Procura ou cria Subtitulo
        Transform subTransform = canvas.transform.Find("SubtitleText");
        TextMeshProUGUI subTmp;
        if (subTransform == null)
        {
            GameObject subObj = new GameObject("SubtitleText");
            subObj.transform.SetParent(canvas.transform, false);
            subTmp = subObj.AddComponent<TextMeshProUGUI>();
        }
        else
        {
            subTmp = subTransform.GetComponent<TextMeshProUGUI>();
        }

        subTmp.text = "TEMPESTADE ESPACIAL INFINITA";
        subTmp.fontSize = 24;
        subTmp.color = new Color(0.4f, 0.85f, 1f, 1f);
        subTmp.alignment = TextAlignmentOptions.Center;
        RectTransform subRt = subTmp.rectTransform;
        subRt.anchorMin = new Vector2(0.5f, 0.5f);
        subRt.anchorMax = new Vector2(0.5f, 0.5f);
        subRt.anchoredPosition = new Vector2(0f, 155f);
        subRt.sizeDelta = new Vector2(600f, 40f);
        EditorUtility.SetDirty(subTmp);

        // Procura ou cria Recorde
        Transform hsTransform = canvas.transform.Find("HighScoreText");
        TextMeshProUGUI hsTmp;
        if (hsTransform == null)
        {
            GameObject hsObj = new GameObject("HighScoreText");
            hsObj.transform.SetParent(canvas.transform, false);
            hsTmp = hsObj.AddComponent<TextMeshProUGUI>();
        }
        else
        {
            hsTmp = hsTransform.GetComponent<TextMeshProUGUI>();
        }

        hsTmp.text = "RECORDE: 0000";
        hsTmp.fontSize = 28;
        hsTmp.color = new Color(1f, 0.85f, 0.2f, 1f);
        hsTmp.alignment = TextAlignmentOptions.Center;
        RectTransform hsRt = hsTmp.rectTransform;
        hsRt.anchorMin = new Vector2(0.5f, 0.5f);
        hsRt.anchorMax = new Vector2(0.5f, 0.5f);
        hsRt.anchoredPosition = new Vector2(0f, -50f);
        hsRt.sizeDelta = new Vector2(400f, 50f);
        EditorUtility.SetDirty(hsTmp);

        // Conecta ao StartButton
        StartButton startBtn = Object.FindFirstObjectByType<StartButton>();
        if (startBtn != null)
        {
            SerializedObject so = new SerializedObject(startBtn);
            so.FindProperty("highScoreText").objectReferenceValue = hsTmp;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Procura ou cria Instrucoes
        Transform instTransform = canvas.transform.Find("InstructionsText");
        TextMeshProUGUI instTmp;
        if (instTransform == null)
        {
            GameObject instObj = new GameObject("InstructionsText");
            instObj.transform.SetParent(canvas.transform, false);
            instTmp = instObj.AddComponent<TextMeshProUGUI>();
        }
        else
        {
            instTmp = instTransform.GetComponent<TextMeshProUGUI>();
        }

        instTmp.text = "[W, A, S, D / Setas] Pilotar   |   [Espaço / Clique] Atirar";
        instTmp.fontSize = 20;
        instTmp.color = new Color(0.85f, 0.85f, 0.9f, 0.9f);
        instTmp.alignment = TextAlignmentOptions.Center;
        RectTransform instRt = instTmp.rectTransform;
        instRt.anchorMin = new Vector2(0.5f, 0f);
        instRt.anchorMax = new Vector2(0.5f, 0f);
        instRt.anchoredPosition = new Vector2(0f, 60f);
        instRt.sizeDelta = new Vector2(800f, 40f);
        EditorUtility.SetDirty(instTmp);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("===> tela inicio.unity configurada com sucesso!");
    }
}
