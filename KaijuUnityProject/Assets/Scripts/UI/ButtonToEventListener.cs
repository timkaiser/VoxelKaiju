using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonToEventListener : MonoBehaviour
{
    #region Variables
    EventSystem system;
    #endregion
    #region Basic Functions
    private void OnEnable()
    {
        system = GameObject.FindObjectOfType<EventSystem>();
        system.firstSelectedGameObject = transform.GetComponentInChildren<Button>().gameObject;
        StartCoroutine(SelectButtonLater(system.firstSelectedGameObject));
    }
    private void OnDisable()
    {
        system.SetSelectedGameObject(null);
    }
    private IEnumerator SelectButtonLater(GameObject button)
    {
        yield return null;
        system.SetSelectedGameObject(button);
    }

    #endregion
}
