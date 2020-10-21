using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Obsolete("This script is Obsolete and should not be used anymore.")]
public class InGameMenuControl : MonoBehaviour
{
    private enum PrevStickPos : byte
    {
        UIUp,
        Null, 
        UiDown
    }

    PrevStickPos prevPos = PrevStickPos.Null;

    VerticalLayoutGroup vgroup;
    [SerializeField]
    int uiObjects = 0;
    [SerializeField]
    Character.PlayerInputManager input;
    [SerializeField]
    int activeIndex = 0;

    [SerializeField]
    Color highlightedColor = Color.red;
    [SerializeField]
    Color defaultColor = Color.black;

    // Start is called before the first frame update
    void OnEnable()
    {
        activeIndex = 0;
        AssignInput();        
    }

    // Update is called once per frame
    void Update()
    {
        bool UIdown = input.getUIDown();
        bool UIUp = input.getUIUp();
        if (UIdown && !prevPos.Equals(PrevStickPos.UiDown))
        {
            activeIndex += 1;
            prevPos = PrevStickPos.UiDown;
        }
        else if (UIUp && !prevPos.Equals(PrevStickPos.UIUp))
        {
            activeIndex -= 1;
            prevPos = PrevStickPos.UIUp;
        }
        else if (!UIUp && !UIdown)
        {
            prevPos = PrevStickPos.Null;
        }
            
        bool UIA = input.getUIA();
            
        int activeChildCount = 0;
        List<Transform> activeChilds = new List<Transform>();
        foreach(Transform t in transform)
        {
            if(t.gameObject.activeInHierarchy)
            {
                activeChilds.Add(t);
                activeChildCount++;
            }
        }
        uiObjects = activeChildCount;
        if (activeIndex >= activeChildCount)
        {
            activeIndex = 0;
        }
        if(activeIndex < 0)
        {
            activeIndex = uiObjects - 1;
        }
            
        for (int i = 0; i < activeChildCount; i++)
        {
            var text = activeChilds[i].GetChild(0).GetComponent<Text>();
            if (text != null)
            {
                if (i == activeIndex)
                {
                    text.color = highlightedColor;
                }
                else
                {
                    text.color = defaultColor;
                }
            }
        }

        if(UIA)
        {
            var button = activeChilds[activeIndex].GetComponent<Button>();
            if (button != null)
                button.onClick.Invoke();
        }  
    }

    private void OnDisable()
    {
        prevPos = PrevStickPos.Null;
    }

    private void AssignInput()
    {
        if (input == null)
        {
            input = Character.PlayerInputManager.Instance;
        }
    }
}
