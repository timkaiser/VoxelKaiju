using Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : MonoBehaviour
{
    [SerializeField]
    private float range = 8f;

    private GameObject godzilla;

    private Car thisCar;

    private AudioSource siren;

    [SerializeField]
    private bool onDuty = false;

    public bool IsOnDuty
    {
        get { return onDuty; }
        set
        {
            onDuty = value;
            if (onDuty)
            {
                thisCar.MaxOutSpeed();
                siren.Play();
            }
            else
            {
                thisCar.ResetSpeedLimit();
                siren.Stop();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        TaskManager.Instance.DestroyedObjectEvent.AddListener(OnObjectDestroyed);
        godzilla = GameObject.FindWithTag("Player");
        thisCar = transform.GetComponent<Car>();
        siren = GetComponents<AudioSource>()[1];
        siren.clip = SoundManager.Instance.GetEffect("PoliceSiren");
    }

    void OnObjectDestroyed()
    {
        if (Vector3.Distance(transform.position, godzilla.transform.position) < range && !IsOnDuty)
        {
            IsOnDuty = true;
            StartCoroutine(SendRadioMessage(10f));
        }
    }

    private void OnDestroy()
    {
        TaskManager.Instance.DestroyedObjectEvent.RemoveListener(OnObjectDestroyed);
    }

    private IEnumerator SendRadioMessage(float time)
    {
        yield return new WaitForSeconds(time);
        thisCar.spawnManager.SetCopsOnDuty(true);
        thisCar.spawnManager.IncreaseCopCount();
    }

    private void Update()
    {
        if (IsChasing())
        {
            thisCar.spawnManager.StartManHunt();
            thisCar.OverrideTurnPos(godzilla.transform.position);
        }
    }

    public bool IsChasing()
    {
        return IsOnDuty && Vector3.Distance(transform.position, godzilla.transform.position) < range * 1.5f;
    }

    public void ToggleSiren(bool on)
    {
        if (on && onDuty) siren.Play();
        else siren.Stop();
    }
}
