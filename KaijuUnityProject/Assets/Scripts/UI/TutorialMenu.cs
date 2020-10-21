using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialMenu : MonoBehaviour
{
    EventSystem system;
    GameObject button;
    private void OnEnable()
    {
        system = GameObject.FindObjectOfType<EventSystem>();
        button = transform.GetComponentInChildren<Button>().gameObject;
        button.SetActive(false);
    }

    public void SetButtonClickable()
    {
        button.SetActive(true);
        system.firstSelectedGameObject = button;
        system.SetSelectedGameObject(button);
    }
}
