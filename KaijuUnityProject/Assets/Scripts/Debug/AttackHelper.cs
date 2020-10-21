using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Character;

public class AttackHelper : MonoBehaviour
{

    public Character.KaijuController character;
    public ComboMaster comboMaster;
    public bool debug = true;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<KaijuController>();
        comboMaster = GetComponent<ComboMaster>();
        if(character == null)
        {
            this.enabled = false;
            Debug.Log("Character not found in Scene, deactivating Attack Helper at: " + this.name);
        }
        if(comboMaster == null)
        {
            this.enabled = false;
            Debug.Log("ComboMaster not found in Scene, deactivating Attack Helper at: " + this.name);
        }
        Debug.Log("Attack helper set up correctly");
    }

    private void OnTriggerEnter(Collider other)
    {
        Destruction dest = other.GetComponent<Destruction>();
        if(dest != null)
        {
            Attack attack = character.GetCurrentAttack();
            if (attack.getAttackID() == 0)
                return;
            comboMaster.RegisterAttack(attack, dest.GetLevel());
            float timing = character.GetCurrentTiming();
            if(debug)
            {
                Debug.Log("Something happened at object: " + this.name);
                Debug.Log("Attacked with " + attack.getAnimationName() + "\n" + "Deals " + attack.getDamage() + " Damage");
                Debug.Log("Attacked with Timing score: " + timing.ToString());
            }
        }
    }
}
