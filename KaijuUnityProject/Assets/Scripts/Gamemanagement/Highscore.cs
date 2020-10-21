using Management;
using UnityEngine;
using UnityEngine.UI;

public class Highscore : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The actual score of the current in-game points. It cannot fall below zero.
    /// </summary>
    [SerializeField]
    private uint score = 0;

    /// <summary>
    /// The multiplier that will affect the calculation when adding points.
    /// </summary>
    private float multiplier = 1;

    /// <summary>
    /// The reference to the text object of the in-game HUD.
    /// </summary>
    private Text hudRef;
    private Multiplier multiplierUI;
    #endregion

    #region Basic Functions
    private void Start()
    {
        GameObject hudPanel = UIManager.Instance.GetHudPanel;
        multiplierUI = hudPanel.GetComponentInChildren<Multiplier>();
        hudRef = hudPanel.transform.Find(this.GetType().Name).GetComponent<Text>();
        UpdateHud();        
    }
    #endregion

    #region Public Functions

    /// <summary>
    /// Increases the current score. The calculation of multipliers etc. will automatically take place in this function.
    /// </summary>
    /// <param name="points">The amount of points that you want to increase the score.</param>
    public void Increase(uint points, bool ignoreMultiplier = false)
    {
        uint plus = 0;
        if(!ignoreMultiplier)
        {
            plus = (uint)(multiplier * points);
        }
        else
        {
            plus = (uint)points;
        }
        score += plus;
        TaskManager.Instance.RegisterAquiredPoints((int)plus);
        UpdateHud();
    }

    /// <summary>
    /// Decreases the current score. The calculation of multipliers etc. will automatically take place in this function.
    /// </summary>
    /// <param name="points">The amount of points that you want to decrease the score.</param>
    public void Decrease(uint points)
    {
        score -= points;
        UpdateHud();
    }

    /// <summary>
    /// Raises the multiplier by a factor of 2;
    /// </summary>
    public void Multiply()
    {
        multiplier *= 2f;
    }

    /// <summary>
    /// Sets the value of the highscore multiplier
    /// </summary>
    /// <param name="value">The new value (e.g. 1.25, 2.0, ...)</param>
    public void ChangeMultiplier(float value) {
        multiplier = value;
        UpdateMultiplierUI();
    }

    /// <summary>
    /// Adds the parameter value to the current multiplier.
    /// </summary>
    /// <param name="value">The value to be added.</param>
    public void AddToMultiplier(float value)
    {
        multiplier += value;
        UpdateMultiplierUI();
    }

    private void UpdateMultiplierUI()
    {
        if(multiplier == 1.5f)
        {
            multiplierUI.SetX1_5();
        }
        else if(multiplier == 2.0f)
        {
            multiplierUI.SetX2();
        }
    }

    /// <summary>
    /// Resets both the score and the multiplier to their initial values zero and one respectively.
    /// </summary>
    public void Reset()
    {
        ResetScore();
        ResetMultiplier();
    }

    /// <summary>
    /// Resets the multiplier to its initial value of one.
    /// </summary>
    public void ResetMultiplier()
    {
        multiplier = 1;
        if(multiplierUI != null)
            multiplierUI.SetX1();
    }

    /// <summary>
    /// Resets the score to its initial value of zero.
    /// </summary>
    public void ResetScore()
    {
        score = 0;
        UpdateHud();
    }
    #endregion

    public int GetScore()
    {
        return (int)score;
    }

    #region Private Functions
    /// <summary>
    /// Updates the text of the HUD with the current score.
    /// </summary>
    private void UpdateHud()
    {
        hudRef.text = score.ToString();
    }
    #endregion
}
