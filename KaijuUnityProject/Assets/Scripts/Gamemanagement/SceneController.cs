using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    /// <summary>
    /// The static instance of the SceneController in the scene.
    /// </summary>
    public static SceneController Instance { get; private set; } = null;

    #region Variables
    /// <summary>
    /// The Canvas that is supposed to be faded to black and back to help covering scene changes.
    /// </summary>
    private CanvasGroup fadeImage = default;

    /// <summary>
    /// The event that is called before a scene is unloaded. Attach listeners to this event if necessary.
    /// </summary>
    public event Action BeforeSceneUnload;

    /// <summary>
    /// The event that is called after a new scene is loaded. Attach listeners to this event if necessary.
    /// </summary>
    public event Action AfterSceneLoad;
    #endregion

    #region Basic Functions
    private void Awake()
    {
        if (Instance == null)                               //Check if instance already exists
            Instance = this;                                //if not, set instance to this        
        else if (Instance != this)                          //If instance already exists and it's not this:     
        {
            Destroy(gameObject);                            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            return;
        }
        DontDestroyOnLoad(gameObject);                      //Sets this to not be destroyed when reloading scene
    }

    private void Start()
    {
        fadeImage = UIManager.Instance.GetFadePanel.GetComponent<CanvasGroup>();
    }

    #endregion

    #region Private Functions

    /// <summary>
    /// Fades the screen to black or back by using a Canvas group alpha.
    /// </summary>
    /// <param name="finalAlpha">Enter 1.0f for a fade to black screen or 0.0f for a fade to the game [transparent].</param>
    /// <param name="fadeDuration">The time in seconds in float that the whole fadeDuration should take.</param>
    /// <returns></returns>
    private IEnumerator Fade(float finalAlpha, float fadeDuration)
    {
        fadeImage.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(fadeImage.alpha - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately(fadeImage.alpha, finalAlpha))
        {
            fadeImage.alpha = Mathf.MoveTowards(fadeImage.alpha, finalAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        if (finalAlpha == 0f) fadeImage.blocksRaycasts = false;
    }

    /// <summary>
    /// Switches the scenes from current scene to new scene by asynchronously loading the new scene and fading an image to black in between.
    /// </summary>
    /// <param name="sceneName">The name of the scene in the Unity scene manager.</param>
    /// <returns>An IEnumerator. Please start this as a Coroutine.</returns>
    private IEnumerator SwitchScenes(string sceneName)
    {
        yield return StartCoroutine(Fade(1f, 1f));
        BeforeSceneUnload?.Invoke();        
        yield return StartCoroutine(LoadScene(sceneName));
        yield return StartCoroutine(Fade(0f, 1f));
    }

    /// <summary>
    /// Loads a new Scene and unloads the previous scene afterwards.
    /// </summary>
    /// <param name="sceneName">The name of the scene in the Unity scene manager.</param>
    /// <returns>An IEnumerator. Please start this as a Coroutine.</returns>
    private IEnumerator LoadScene(string sceneName)
    {
        yield return StartCoroutine(Fade(1f, 1f));
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);        
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
        AfterSceneLoad?.Invoke();
        yield return StartCoroutine(Fade(0f, 1f));
    }
    #endregion

    #region Public Functions


    /// <summary>
    /// Fades and Loads the main menu.
    /// </summary>
    public void ToMenuScene()
    {
        StartCoroutine(SwitchScenes("MainMenu"));
    }

    /// <summary>
    /// Fades and loads the game scene.
    /// </summary>
    public void ToGameScene(string name)
    {
        StartCoroutine(LoadScene(name));
    }

    /// <summary>
    /// Fades and loads the game scene.
    /// </summary>
    public void ReloadGameScene()
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name));
    }

    /// <summary>
    /// Fades and loads the end menu scene.
    /// </summary>
    public void ToEndScene()
    {
        StartCoroutine(SwitchScenes(Config.GetString(Config.Type.Scenemanager, "EndScene")));
    }
    #endregion
}
