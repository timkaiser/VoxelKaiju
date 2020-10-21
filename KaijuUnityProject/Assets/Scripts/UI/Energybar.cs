using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Energybar : MonoBehaviour
{
    public Image bar;
    private float value = 0;
    Character.KaijuController controller;

    private void OnEnable()
    {
        controller = FindObjectOfType<Character.KaijuController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(controller != null)
        {
            value = controller.GetEnergy();
            bar.fillAmount = value / 100.0f;
        }
    }
}
