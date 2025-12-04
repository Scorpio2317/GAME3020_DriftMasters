using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MapSelectiionScene");
    }

    public void RaceTrackSnowMountain()
    {
        SceneManager.LoadScene("GameMap1Scene");
    }

    public void RaceTrackMountain()
    {
        SceneManager.LoadScene("GameMap2Scene 1");
    }

    public void ExitGame()
    {
        Application.Quit();

        //UnityEditor.EditorApplication.isPlaying = false;
    }
}
