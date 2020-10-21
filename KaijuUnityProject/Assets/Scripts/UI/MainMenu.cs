using Management;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// The button prefab that will be spawned as a button for each game level.
    /// </summary>
    [SerializeField]
    private GameObject buttonPrefab;

    private EventSystem system;

    #region Basic Functions
    private void OnEnable()
    {
        Initiate();
    }
    private void OnDisable()
    {
        DeleteButtons();
    }

    private void Start()
    {
        Initiate();
    }

    /// <summary>
    /// Loads buttons and sets the EventSystem to select the first button.
    /// </summary>
    private void Initiate()
    {
        bool buttonsExist = false;
        if (transform.childCount.Equals(0)) buttonsExist = CreateButtons();
        else buttonsExist = true;
        //Sets the first selected button to button 1 => Controller can now handle input without extra code
        if (buttonsExist)
        {
            system = GameObject.FindObjectOfType<EventSystem>();
            system.firstSelectedGameObject = transform.GetChild(0).gameObject;
            system.SetSelectedGameObject(system.firstSelectedGameObject);
        }
    }
    #endregion

    /// <summary>
    /// Creates buttons and adds them as children depending on the amount of Levels loaded.
    /// </summary>
    /// <returns>True if buttons were created, false if not.</returns>
    private bool CreateButtons()
    {
        bool buttonCreated = false;
        for (int i = 0; i < LevelList.size; i++)
        {
            Button button = Instantiate(buttonPrefab).GetComponent<Button>();
            Level level = LevelList.GetLevelById(i);
            button.transform.GetComponentInChildren<Text>().text = level.Title;
            button.onClick.AddListener(() => GameManager.Instance.LoadGame(level.Title));
            //button.onClick.AddListener(() => UIManager.Instance.GetTutorialPanel.SetActive(true));
            button.onClick.AddListener(() => gameObject.SetActive(false));
            button.transform.SetParent(transform);
            buttonCreated = true;
        }
        return buttonCreated;
    }

    /// <summary>
    /// Deletes all button children.
    /// </summary>
    private void DeleteButtons()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Button button = transform.GetChild(i).GetComponent<Button>();
            int myID = i;
            button.onClick.RemoveAllListeners();
            Destroy(button.gameObject);
        }
    }
}
