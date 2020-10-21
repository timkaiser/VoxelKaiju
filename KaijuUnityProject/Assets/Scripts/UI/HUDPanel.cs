using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDPanel : MonoBehaviour
{
    /// <summary>
    /// Pauses the game when the menu button in the in-game HUD is clicked.
    /// </summary>
    public void Pause()
    {
        GameManager.Instance.PauseGame();
    }
}
