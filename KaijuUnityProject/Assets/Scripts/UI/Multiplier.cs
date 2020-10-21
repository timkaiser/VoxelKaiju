using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Multiplier : MonoBehaviour
{
    public Sprite empty;
    public Sprite spriteX1_5;
    public Sprite spriteX2;

    Image image;

    // Start is called before the first frame update
    void OnEnable()
    {
        this.gameObject.SetActive(true);
        image = GetComponent<Image>();
        SetX1();
    }

    public void SetX1()
    {
        image.sprite = empty;
        image.color = new Vector4(image.color.r, image.color.g, image.color.b, 0.0f);
    }

    public void SetX1_5()
    {
        image.sprite = spriteX1_5;
        image.color = new Vector4(image.color.r, image.color.g, image.color.b,1.0f);
    }

    public void SetX2()
    {
        image.sprite = spriteX2;
        new Vector4(image.color.r, image.color.g, image.color.b, 1.0f);

    }
}
