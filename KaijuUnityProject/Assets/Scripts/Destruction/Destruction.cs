using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Management;

/* HEADER:
 * This script can be added to GameObjects to make them destructable when clicked on. 
 * It needs a gameobject containing prefabs of the broken parts as parameter.
 */
public class Destruction : MonoBehaviour {
    public GameObject broken_version;      // prefabs of the parts the object breaks into. On destruction Object is replaced with this object

    public bool debug_destroy = false;

    // health related attributes
    public float max_health = 100.0f;
    [SerializeField]
    private float current_health;
    private float regeneration = 0.0f;      //health regeneration per second
    public uint reward = 10;
    [SerializeField]
    private uint level = 0;
    [SerializeField]
    private uint destroyOnContactRequiredLevel = 3;

    private float force = 200.0f;

    float fireStormTimer = 0.5f;

    // type of destructible
    public DestructibleData.DestructibleType type = 0;

    private void Start()
    {
        current_health = max_health;  
    }

    //This methode is called once per frame. Here it handles regeneration
    private void Update() {
        if (debug_destroy) { DamageObject(1000); debug_destroy = false; }
        //regeneration
        if(current_health < max_health) {
            current_health += Mathf.Clamp(regeneration * Time.deltaTime, 0, max_health-current_health);
        }
    }


    //This methode "breaks" the object by replacing it with a already broken version
    //IN: float damage: the amount of damage done to the object
    //COMMENT: Ich hab die hier mal auf public gesetzt um vom Charakter aus dran zu kommen.
    public void DamageObject(float damage)
    {
        current_health -= damage;
        //TODO (FUTURE) : let some voxel sparkle animation run at this point
        if (current_health <= 0) {
            BreakObject();
        }
        this.GetComponent<Renderer>().material.SetFloat("_BreakThreshold", current_health / max_health);
    }
    
    //TODO: add another parameter that describes the collision vector in order to bounce objects away
    public float DamageByKaiju(float damage, Vector3 position /*for force direction*/)
    {
        current_health -= damage;
        //TODO (FUTURE) : let some voxel sparkle animation run at this point
        if (current_health <= 0)
        {
            BreakObject(this.transform.position - position);
            GameManager.Instance.GetComponent<Highscore>().Increase(reward);
            //Tell the TaskManager that an object got destroyed 
            TaskManager.Instance.RegisterDestroyedObject(type);
        }
        this.GetComponent<Renderer>().material.SetFloat("_BreakThreshold", current_health / max_health);
        //return the damage dealt to object
        return Mathf.Max(-current_health, 0.0f);
    }

    //This methode "breaks" the object by replacing it with a already broken version
    private void BreakObject() {
        BreakObject(Vector3.zero);
    }

    //This methode "breaks" the object by replacing it with a already broken version and gives it a random force
    private void BreakObject(Vector3 force_direction) {
        // instantiate broken version
        GameObject broken_instance = Instantiate(broken_version, transform.position, transform.rotation);
        broken_instance.transform.localScale = transform.localScale;
        Rigidbody[] parts = broken_instance.GetComponentsInChildren<Rigidbody>();

        //add force to each part to make destruction more dynamic
        foreach (Rigidbody part in parts) {
            part.AddExplosionForce(100, transform.position, 5);
            if(force_direction != Vector3.zero) {
                part.AddForce(force_direction.normalized * force);
            }
        }

        //destroy old object
        Destroy(this.gameObject);
    }

    public void OnFireStorm(float deltaTime, float damage, Vector3 position)
    {
        fireStormTimer -= deltaTime;
        if(fireStormTimer <= 0.0f)
        {
            fireStormTimer = 0.5f;
            DamageByKaiju(damage, position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var character = other.gameObject.GetComponent<Character.KaijuController>();
        if(character != null)
        {
            Debug.Log("character touching");
            if (character.GetGrowthLevel() >= destroyOnContactRequiredLevel)
            {

                BreakObject();
                GameManager.Instance.GetComponent<Highscore>().Increase(reward / 2,true);
            }
        }
    }

    public uint GetLevel()
    {
        return level;
    }
}
