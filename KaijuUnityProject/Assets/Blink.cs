using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blink : MonoBehaviour
{
    private Text text;
    private Color color;
    public Color blinkColor;

    private void OnEnable()
    {
        text = gameObject.GetComponent<Text>();
        color = text.color;
        StartCoroutine(Blinking());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        text.color = color;
    }

    private IEnumerator Blinking()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.75f);           
            text.color = blinkColor;
            yield return new WaitForSeconds(0.75f);
            text.color = color;
        }
    }
}
