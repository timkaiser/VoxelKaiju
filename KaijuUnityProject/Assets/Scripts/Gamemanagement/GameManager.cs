using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Management;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    /// <summary>
    /// The static instance of the GameManager in the scene.
    /// </summary>
    public static GameManager Instance { get; private set; } = null;

    public enum GameStates { None, Title, Menu, Paused, Playing, Scores }
    public UnityEvent GameStateSwitched;
    public UnityEvent RestartGameEvent;

    #region Variables
    private Timer timer;
    private Highscore highscore;
    private ProceduralGenerator generator;
    private Character.CharacterCamera camera;
    private GameStates gameState = GameStates.None;

    public GameStates GameState {
        get {
        return gameState;
        }
        set
        {
            gameState = value;
            GameStateSwitched?.Invoke();
        }
    }
    #endregion

    #region Basic Functions
    private void Awake()
    {
        //Check if instance is assigned
        if (Instance == null) Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        //Assign references to scripts attached to this gameObject
        timer = GetComponent<Timer>();
        highscore = GetComponent<Highscore>();
        generator = GetComponent<ProceduralGenerator>();

        if (RestartGameEvent == null)
            RestartGameEvent = new UnityEvent();

    }

    private void Start()
    {
        Tools.Console.RegisterCommand(Tools.Console.Category.Gameplay, "Win", WinCommand);
        //Note: do not run StartGame() from here as not all Instances might be done initializing.
        InitializeGame();        
    }

    private void OnDisable()
    {
        SceneController.Instance.AfterSceneLoad -= OnLevelFinishedLoading;
    }
    #endregion

    #region Private Functions 
    private void OnLevelFinishedLoading()
    {
        SceneController.Instance.AfterSceneLoad -= OnLevelFinishedLoading;

        ActiveLevel level = LevelManager.Instance.GetActiveLevel();
        TaskManager.Instance.Init(level);
        //Only generate a map when it's not the demo Scene.
        if (!LevelManager.Instance.GetActiveLevelTitle().Equals("Demo"))
        {
            generator.DefineMapSettings(level);
            generator.Generate();
        }  
        
        GameObject tutPanel = UIManager.Instance.GetTutorialPanel;
        if (tutPanel.activeSelf) tutPanel.GetComponent<TutorialMenu>().SetButtonClickable();
        else StartGame();
    }

    private void ResetCounters()
    {
        highscore.Reset();
        timer.Reset();
    }
    #endregion

    #region Public Functions

    /// <summary>
    /// Initialize things like map generation, values etc.
    /// </summary>
    public void InitializeGame()
    {
        LevelList.Init();
        //Set Gamestate
        GameState = GameStates.Title;
    }

    public void LoadGame(string title)
    {
        SceneController.Instance.AfterSceneLoad += OnLevelFinishedLoading;
        if (title.Equals("Demo")) SceneController.Instance.ToGameScene(title);
        else SceneController.Instance.ToGameScene("Level");
        LevelManager.Instance.LoadLevel(title);
    }

    /// <summary>
    /// Starts the game. Timer will start counting and character will be able to move.
    /// </summary>
    public void StartGame()
    {
        ResetCounters();
        ResumeGame();
    }

    public void ReloadGame()
    {
        ResetCounters();
        SceneController.Instance.AfterSceneLoad += OnLevelFinishedLoading;
        SceneController.Instance.ReloadGameScene();        
        RestartGameEvent.Invoke();
        GameState = GameStates.Paused;
    }

    /// <summary>
    /// Pauses the game. This function stops input and interaction with the character.
    /// </summary>
    public void PauseGame()
    {
        timer.SetInactive();
        //Time.timeScale = 0;
        GameState = GameStates.Paused;
        UIManager.Instance.GetHudPanel.SetActive(false);
        UIManager.Instance.GetInGameMenuPanel.transform.Find("Continue Button").gameObject.SetActive(true);
        UIManager.Instance.GetInGameMenuPanel.SetActive(true);        
    }

    /// <summary>
    /// Resumes the game if paused. Control over the character will again be granted to the player.
    /// </summary>
    public void ResumeGame()
    {
        timer.SetActive();
        //Time.timeScale = 1;
        GameState = GameStates.Playing;
        UIManager.Instance.GetInGameMenuPanel.SetActive(false);
        UIManager.Instance.GetInGameMenuPanel.transform.Find("Continue Button").gameObject.SetActive(false);
        UIManager.Instance.GetHudPanel.SetActive(true);
    }

    /// <summary>
    /// Ends the game. Do everything necessary to move on to the end screen/ open the end menu.
    /// </summary>
    public void FinishGame(bool hasWon)
    {
        timer.SetInactive();
        if (hasWon)
        {
            Debug.Log("Game won!");
            highscore.Increase((uint)timer.GetTimeLeft() * 100, true);
            SoundManager.Instance.PlayEFX("Win");
        }
        else
        {
            Debug.Log("Game over.");
            SoundManager.Instance.PlayEFX("Lose");
        }
        string scenename = SceneManager.GetActiveScene().name;
        GameState = GameStates.Scores;
        if (scenename == "Level")
        {
            UIManager.Instance.GetEnterPlayerDisplay.SetActive(true);
            
        }
        else
        {
            UIManager.Instance.GetInGameMenuPanel.SetActive(true);
        }
        RestartGameEvent.Invoke();
    }

    public void InvertCameraVerticalAxis()
    {
        if(camera == null)
        {
            camera = FindObjectOfType<Character.CharacterCamera>();
        }
        camera.invertCameraVertical = !camera.invertCameraVertical;
    }

    private void OnApplicationQuit()
    {
        UIManager.Instance.GetHighscorePanel.GetComponent<HighscoreUI>().Save();
    }

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public void ReturnToMenu()
    {
        GameState = GameStates.Title;
        ResetCounters();
        SceneController.Instance.ToMenuScene();
    }

    /// <summary>
    /// Quits the game and closes the application.
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif
    }
    #endregion

    #region Commands
    /// <summary>
    /// Command for Winning the game instantly
    /// </summary>
    /// <param name="status">Don't fill in anything</param>
    public void WinCommand(object[] status)
    {
        SceneController.Instance.ToEndScene();
    }
    #endregion
}
