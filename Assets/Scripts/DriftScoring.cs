using TMPro;
using UnityEngine;

public class DriftScoring : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    [Header("Drift Settings")]
    public float minDriftSlip = 0.5f;

    public float baseScorePerSecond = 50f;

    public float comboWindowDuration = 1.0f;

    [Header("References")]
    public WheelAlignment[] allWheels;

    private float totalScore = 0f;
    private float currentDriftTime = 0f;
    private float comboMultiplier = 1f;
    private float comboTimer = 0f;
    private bool isDrifting = false;

    public float TotalScore => totalScore;

    private void Start()
    {
        UpdateScoreDisplay();
        UpdateComboDisplay();
    }

    private void FixedUpdate()
    {
        CheckForDrift();
        UpdateScoring();
    }

    private void CheckForDrift()
    {
        int driftingWheelCount = 0;
        float totalSlip = 0f;

        // Check all wheels for significant slip
        foreach (var wa in allWheels)
        {
            if (wa.wheelCol.GetGroundHit(out WheelHit hit))
            {
                float slip = Mathf.Abs(hit.sidewaysSlip);
                if (slip > minDriftSlip)
                {
                    driftingWheelCount++;
                    totalSlip += slip;
                }
            }
        }

        // Determine if we are actively drifting
        if (driftingWheelCount > 0)
        {
            isDrifting = true;
            float avgSlip = totalSlip / driftingWheelCount;
            AddDriftPoints(avgSlip);
            comboTimer = comboWindowDuration;
        }
        else
        {
            isDrifting = false;
        }
    }

    private void UpdateScoring()
    {
        if (isDrifting)
        {

        }
        else
        {
            if (comboTimer > 0)
            {
                comboTimer -= Time.fixedDeltaTime;
                if (comboTimer <= 0)
                {
                    ResetCombo();
                }
            }
        }

        UpdateComboDisplay();
    }

    private void AddDriftPoints(float slipIntensity)
    {
        float scoreThisFrame = baseScorePerSecond * Time.fixedDeltaTime * slipIntensity;

        comboMultiplier += Time.fixedDeltaTime * 0.5f;

        comboMultiplier = Mathf.Clamp(comboMultiplier, 1f, 10f);

        totalScore += scoreThisFrame * comboMultiplier;
        UpdateScoreDisplay();
    }

    private void ResetCombo()
    {
        comboMultiplier = 1f;
        UpdateComboDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {(int)totalScore}";
        }
    }

    private void UpdateComboDisplay()
    {
        if (comboText != null)
        {
            if (comboMultiplier > 1f)
            {
                comboText.text = $"x{comboMultiplier:0.0} COMBO";
            }
            else
            {
                comboText.text = "";
            }
        }
    }
}
