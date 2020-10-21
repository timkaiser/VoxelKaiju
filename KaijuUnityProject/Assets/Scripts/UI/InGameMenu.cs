using UnityEngine;
using static GameManager;

public class InGameMenu : MonoBehaviour
{
    #region Basic Functions
    private void OnEnable()
    {
        if (GameManager.Instance.GameState.Equals(GameStates.Paused))
        {
            transform.Find("Continue Button").gameObject.SetActive(true);
            transform.Find("Invert Camera Button").gameObject.SetActive(true);
        }
    }
    private void OnDisable()
    {
        transform.Find("Continue Button").gameObject.SetActive(false);
        transform.Find("Invert Camera Button").gameObject.SetActive(false);
    }

    #endregion

    #region Public Functions
    /// <summary>
    /// Starts the game when the Start button in the In-game menu is clicked.
    /// </summary>
    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }
    /// <summary>
    /// Restarts the game when the restart button in the In-game menu is clicked.
    /// </summary>
    public void Restart()
    {
        GameManager.Instance.ReloadGame();
    }

    public void InvertCameraVerticalAxis()
    {
        GameManager.Instance.InvertCameraVerticalAxis();
    }

    /// <summary>
    /// Resumes the game when the resume button in the In-game menu is clicked.
    /// </summary>
    public void Resume()
    {
        GameManager.Instance.ResumeGame();
    }

    /// <summary>
    /// Returns to the main menu scene.
    /// </summary>
    public void ToMenu()
    {
        GameManager.Instance.ReturnToMenu();
    }
    #endregion
}
