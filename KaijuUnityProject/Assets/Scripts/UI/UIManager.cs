using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// The static instance of the UIManager in the scene. It references all UI panels, so they can be found easier by the other scripts.
    /// </summary>
    public static UIManager Instance { get; private set; } = null;

    #region References
    /// <summary>
    /// The reference to the fade panel. EXAMPLE.
    /// </summary>
    private GameObject fadePanel = default;

    /// <summary>
    /// Get the reference to the fade panel.
    /// </summary>
    public GameObject GetFadePanel
    {
        get
        {
            return fadePanel;
        }
    }

    /// <summary>
    /// The reference to the in-game HUD panel.
    /// </summary>
    private GameObject hudPanel = default;

    /// <summary>
    /// Get the reference to the fade panel.
    /// </summary>
    public GameObject GetHudPanel
    {
        get
        {
            return hudPanel;
        }
    }

    /// <summary>
    /// The reference to the in-game end panel after the match ended.
    /// </summary>
    private GameObject inGameMenuPanel = default;

    /// <summary>
    /// Get the reference to the in-game end panel.
    /// </summary>
    public GameObject GetInGameMenuPanel
    {
        get
        {
            return inGameMenuPanel;
        }
    }

    /// <summary>
    /// The reference to the main menu panel.
    /// </summary>
    private GameObject mainMenuPanel = default;

    /// <summary>
    /// Get the reference to the main menu panel.
    /// </summary>
    public GameObject GetMainMenuPanel
    {
        get
        {
            return mainMenuPanel;
        }
    }

    /// <summary>
    /// The referene to the tutorial screen panel.
    /// </summary>
    private GameObject tutorialPanel;

    /// <summary>
    /// Get the reference to the tutorial panel.
    /// </summary>
    public GameObject GetTutorialPanel
    {
        get
        {
            return tutorialPanel;
        }
    }

    /// <summary>
    /// The reference to the title image panel.
    /// </summary>
    private GameObject titlePanel;

    /// <summary>
    /// Get the reference to the title panel.
    /// </summary>
    public GameObject GetTitlePanel
    {
        get
        {
            return titlePanel;
        }
    }

    /// <summary>
    /// Displays the top 5 highscores
    /// </summary>
    public GameObject GetHighscorePanel
    {
        get
        {
            return highscorePanel;
        }
    }
    private GameObject highscorePanel = default;

    /// <summary>
    /// Displays the top 5 highscores
    /// </summary>
    public GameObject GetEnterPlayerDisplay
    {
        get
        {
            return enterPlayerDisplay;
        }
    }
    private GameObject enterPlayerDisplay = default;


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

        //Get all references to child canvasses
        fadePanel = this.transform.Find("FadePanel").gameObject;
        hudPanel = this.transform.Find("HudPanel").gameObject;
        inGameMenuPanel = this.transform.Find("InGameMenuPanel").gameObject;
        mainMenuPanel = this.transform.Find("MainMenuPanel").gameObject;
        tutorialPanel = this.transform.Find("TutorialDisplay").gameObject;
        highscorePanel = this.transform.Find("HighscoreDisplay").gameObject;
        enterPlayerDisplay = this.transform.Find("EnterNameDisplay").gameObject;
        titlePanel = this.transform.Find("TitlePanel").gameObject;
        //Assert that all sub UI panels were found
        Assert.IsNotNull(fadePanel, "The FadePanel could not be found by the UiManager.");
        Assert.IsNotNull(hudPanel, "The HudPanel could not be found by the UiManager.");
        Assert.IsNotNull(inGameMenuPanel, "The InGameMenuPanel could not be found by the UiManager.");
        Assert.IsNotNull(mainMenuPanel, "The MainMenuPanel could not be found by the UiManager.");
        Assert.IsNotNull(tutorialPanel, "The TutorialPanel could not be found by the UiManager.");
        Assert.IsNotNull(highscorePanel, "The HighscorePanel could not be found by the UiManager.");
        Assert.IsNotNull(enterPlayerDisplay, "The EnterPlayerDisplay could not be found by the UiManager.");
        Assert.IsNotNull(titlePanel, "The TitlePanel could not be found by the UiManager.");
    }

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Equals("MainMenu"))
        {
            titlePanel.SetActive(true);
        }
        else
        {
            GameManager.Instance.LoadGame(sceneName);
            tutorialPanel.SetActive(true);
        }
        SceneController.Instance.AfterSceneLoad += EnableUI;
    }

    private void OnDisable()
    {
        SceneController.Instance.AfterSceneLoad -= EnableUI;
    }
    #endregion

    #region Private Functions
    private void EnableUI()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Equals("MainMenu"))
        {
            mainMenuPanel.SetActive(true);
        }
        else tutorialPanel.SetActive(true);
    }
    #endregion

    #region Public Functions
    #endregion
}
