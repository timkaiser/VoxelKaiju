using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleContinue : MonoBehaviour
{
    Character.PlayerInputManager input;
    Button okButton;

    float cooldown;

    // Start is called before the first frame update
    void Awake()
    {
        cooldown = 0.5f;
        okButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown < 0.0f)
        {
            if (input == null)
            {
                input = Character.PlayerInputManager.Instance;
            }

            else if (input.getUIA())
            {
                okButton.onClick.Invoke();
            }
        }
        else
        {
            cooldown -= Time.deltaTime;
        }
        
    }
}
