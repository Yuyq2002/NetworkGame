using TMPro;
using UnityEngine;

public class ScoreTextUpdate : MonoBehaviour
{
    private TextMeshProUGUI m_TextMeshProUGUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
        GameManager.Instance.OnScoreChanged += UpdateText;
    }

    public void OnDestroy()
    {
        GameManager.Instance.OnScoreChanged -= UpdateText;
    }

    void UpdateText(int[] p_score)
    {
        string text = "";

        for(int i = 0; i < p_score.Length; i++)
        {
            text += p_score[i].ToString();

            if (i != p_score.Length - 1) text += " - ";
        }

        m_TextMeshProUGUI.text = text;
    }
}
