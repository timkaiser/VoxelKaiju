using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStorm : MonoBehaviour
{
    [SerializeField]
    float damagePerTick = 50f;
    Character.KaijuController controller;
    ParticleSystem ps;

    private void Awake()
    {
        controller = FindObjectOfType<Character.KaijuController>();
        ps = GetComponent<ParticleSystem>();
    }

    private void OnTriggerStay(Collider other)
    {
        Destruction script = other.gameObject.GetComponent<Destruction>();
        if (script != null)
        {
           script.OnFireStorm( Time.deltaTime, damagePerTick * controller.getDamageFactor(), controller.transform.position);
        }
       
    }

    public void Pause()
    {
        if(ps != null)
            ps.Pause();
    }

    public void Resume()
    {
        if (ps != null)
            ps.Play();
    }
}
