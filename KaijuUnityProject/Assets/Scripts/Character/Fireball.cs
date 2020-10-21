using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    float energyUsed;
    float damage;

    [SerializeField]
    float speed = 1.0f;
    [SerializeField]
    float damagePerEnergyUsed = 10f;
    [SerializeField]
    float rangeFactor = 1.0f;
    Vector3 direction;
    Vector3 initalScale;
    Vector3 initialPos;

    bool markedForDestroy = false;

    // Start is called before the first frame update
    void Awake()
    {
        energyUsed = 0;
        damage = 0;
        //direction = Vector3.zero;
        initalScale = this.transform.localScale;
        initialPos = this.transform.position;
    }

    public void InitializeFireball(Vector3 dir, float energyUsed, float multiplier)
    {
        direction = dir;
        this.energyUsed = energyUsed;
        
        damage = energyUsed * damagePerEnergyUsed * multiplier;
        Debug.Log(damage);
    }

    // Update is called once per frame
    void Update()
    {
        if(markedForDestroy)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        rangeFactor = transform.localScale.x * 4f;

        transform.Translate(direction * speed * Time.deltaTime, Space.Self);
        if(energyUsed != 0 && Vector3.Distance(transform.position, initialPos) > rangeFactor * energyUsed)
        {
            DestroyImmediate(this.gameObject);
        }
        if (energyUsed != 0 && damage <= 0)
        {
            DestroyImmediate(this.gameObject);
        }
    }

    public void Increase(float deltaTime)
    {
        this.transform.localScale = initalScale * Mathf.Max(deltaTime, 1.0f) * 0.5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destruction script = other.gameObject.GetComponent<Destruction>();
        if (script != null)
        {
            float attackDamage = damage;
            damage = script.DamageByKaiju(attackDamage, initialPos);
        }       
    }

    private void OnTriggerExit(Collider other)
    {
        Destruction script = other.gameObject.GetComponent<Destruction>();
        if (script != null)
        {
            float attackDamage = damage;
            damage = script.DamageByKaiju(attackDamage, initialPos);
        }
    }

    public void markDestroy()
    {
        markedForDestroy = true;
    }
}
