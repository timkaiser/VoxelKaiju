using Management;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Determines whether the script should count down the timer every update or not.
    /// </summary>
    [SerializeField]
    private bool isActive = false;

    /// <summary>
    /// The time a match will take.
    /// </summary>
    private float timer = 5f*60.0f;

    /// <summary>
    /// The reference to the text object of the in-game HUD.
    /// </summary>
    private Text hudRef;
    #endregion

    #region Basic Functions

    private void Start()
    {
        hudRef = UIManager.Instance.GetHudPanel.transform.Find(this.GetType().Name).GetComponent<Text>();
        UpdateHUD();
    }

    void Update()
    {
        if (isActive)
        {
            timer -= Time.deltaTime;
            UpdateHUD();
            if (timer <= 0.0f)
            {
                GameManager.Instance.FinishGame(false);
            }
        } 
    }
    #endregion

    #region Public Functions

    /// <summary>
    /// Starts the timer to count down.
    /// </summary>
    public void SetActive()
    {
        isActive = true;
    }

    /// <summary>
    /// Stops the timer from counting down.
    /// </summary>
    public void SetInactive()
    {
        isActive = false;
    }

    /// <summary>
    /// Resets the timer to its initial value and stops counting down.
    /// </summary>
    public void Reset()
    {
        ActiveLevel active = LevelManager.Instance.GetActiveLevel();
        int value = Config.GetInt(Config.Type.Gameplay, "Timer");
        if (active != null) value = active.time;
        timer = value;
        SetInactive();
    }

    /// <summary>
    /// Set the timer to a specific time that a match should take.
    /// </summary>
    /// <param name="time">The length of one match.</param>
    public void SetTime(float time)
    {
        timer = time;
    }

    /// <summary>
    /// Get the time left.
    /// </summary>
    /// <returns>Returns the time left as float.</returns>
    public float GetTimeLeft()
    {
        return timer;
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Updates the text of the HUD with the current time.
    /// </summary>
    private void UpdateHUD()
    {
        hudRef.text = Mathf.CeilToInt(timer).ToString();
    }
    #endregion
}
