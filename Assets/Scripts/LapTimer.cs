using TMPro;
using UnityEngine;

public class LapTimer : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;

    [Header("Settings")]
    public int totalLaps = 3;
    private float currentTime = 0f;
    private bool isRacing = true;

    private void Update()
    {
        if (isRacing)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
