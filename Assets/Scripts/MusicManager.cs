using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private string[] menuScenes = { "MainMenuScene", "MapSelectiionScene", "ResultsScene" };

    private AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isMenu = false;

        foreach (var menuName in menuScenes)
        {
            if (scene.name == menuName)
            {
                isMenu = true;
                break;
            }
        }

        if (isMenu)
        {
            if (!musicSource.isPlaying)
                musicSource.Play();
        }
        else
        {
            if (musicSource.isPlaying)
                musicSource.Stop();
        }
    }
}
