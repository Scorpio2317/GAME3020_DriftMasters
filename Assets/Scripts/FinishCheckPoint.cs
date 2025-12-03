using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishCheckPoint : MonoBehaviour
{
    [Header("References")]
    public LapTimer lapTimer;
    public DriftScoring driftScoring;

    private bool hasFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb == null) 
            return;

        if (!rb.CompareTag("Player")) 
            return;

        if (hasFinished) 
            return;

        hasFinished = true;

        // Stop timer + save time
        if (lapTimer != null)
        {
            lapTimer.StopTimer();
            RaceResultData.finalTime = lapTimer.CurrentTime;
        }

        // Save score
        if (driftScoring != null)
        {
            RaceResultData.finalScore = driftScoring.TotalScore;
        }

        Debug.Log("FinishCheckPoint: loading scene " + "ResultsScene");
        SceneManager.LoadScene("ResultsScene");
    }
}
