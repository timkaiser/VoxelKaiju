using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPitch : MonoBehaviour
{
    private void Awake()
    {
        AudioSource source = GetComponent<AudioSource>();
        SoundManager.Instance.RandomizePitch(source);
        source.Play();
    }
}
