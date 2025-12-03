using TMPro;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        int minutes = Mathf.FloorToInt(RaceResultData.finalTime / 60f);
        int seconds = Mathf.FloorToInt(RaceResultData.finalTime % 60f);

        if (timeText != null)
        {
            timeText.text = $"TIME: {minutes:00}:{seconds:00}";
        }

        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {(int)RaceResultData.finalScore}";
        }
    }
}
