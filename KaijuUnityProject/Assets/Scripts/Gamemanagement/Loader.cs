using UnityEngine.Assertions;
using UnityEngine;
using Management;
using Character;
using UnityEngine.EventSystems;

/// <summary>
/// This script should be attached to the camera in each scene. It will automatically load the necessary objects
/// for the game and instantiate them as statics that can be accessed anywhere.
/// </summary>
public class Loader : MonoBehaviour
{
    #region Prefab References
    /// <summary>
    /// Reference to the GameManager prefab gameObject that shall be instantiated.
    /// </summary>
    [SerializeField]
    private GameObject gameManager;

    /// <summary>
    /// Reference to the SceneManager prefab gameObject that shall be instantiated.
    /// </summary>
    [SerializeField]
    private GameObject sceneController;
   
    /// <summary>
    /// Reference to the SoundManager prefab gameObject that shall be instantiated.
    /// </summary>
    [SerializeField]
    private GameObject soundManager;

    /// <summary>
    /// Reference to the UIManager prefab gameObject that shall be instantiated.
    /// </summary>
    [SerializeField]
    private GameObject uiManager;

    /// <summary>
    /// Reference to the LevelManager prefab gameObject that shall be instantiated.
    /// </summary>
    [SerializeField]
    private GameObject levelManager;

    /// <summary>
    /// Reference to the TaskManager prefab gameObject that shall be instantiated.
    /// </summary>
    [SerializeField]
    private GameObject taskManager;

    /// <summary>
    /// Reference to the InputManager prefab gameObject that shall be instantiated.
    /// </summary>
    [SerializeField]
    private GameObject inputManager;

    /// <summary>
    /// Reference to the EventSystem prefab that shall be instantiated.
    /// </summary>
    [SerializeField]
    private GameObject eventSystem;
    #endregion

    #region Basic Functions

    /// <summary>
    /// On Awake instantiate all non exitent prefabs as static variables
    /// </summary>
    private void Awake()
    {
        //Assert every prefab is assigned
        Assert.IsNotNull(gameManager, "The GameManager prefab is not set in the loader.");
        Assert.IsNotNull(sceneController, "The SceneManager prefab is not set in the loader.");
        Assert.IsNotNull(soundManager, "The SoundManager prefab is not set in the loader.");
        Assert.IsNotNull(uiManager, "The UIManager prefab is not set in the loader.");
        Assert.IsNotNull(levelManager, "The LevelManager prefab is not set in the loader.");
        Assert.IsNotNull(taskManager, "The TaskManager prefab is not set in the loader.");
        Assert.IsNotNull(inputManager, "The TaskManager prefab is not set in the loader.");
        Assert.IsNotNull(eventSystem, "The EventSystem prefab is not set in the loader.");

        //Check if instances have already been assigned to static variables or if it's still null
        EventSystem system = GameObject.FindObjectOfType<EventSystem>();
        if (system == null)
        {
            Instantiate(eventSystem);
        }
        if (GameManager.Instance == null)
        {
            Instantiate(gameManager);
        }
        if (SceneController.Instance == null)
        {
            Instantiate(sceneController);
        }
        if (SoundManager.Instance == null)
        {
            Instantiate(soundManager);
        }
        if (UIManager.Instance == null)
        {
            Instantiate(uiManager);
        }
        if (LevelManager.Instance == null)
        {
            Instantiate(levelManager);
        }
        if (TaskManager.Instance == null)
        {
            Instantiate(taskManager);
        }
        if (PlayerInputManager.Instance == null)
        {
            Instantiate(inputManager);
        }
    }

    #endregion
}
