using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// This Enum maps to the clip names!
/// The Name of the attack clips must match with the enum names to map to metadata
/// </summary>
public enum AttackID
{
    None = 0,
    Punch = 1,
    PunchFlipped = 2,
    swipe = 3,  
    SwipeFlipped = 4,
    jumpattack2 = 5,
    kick = 6,
    kickFlipped = 7,
    strikeForward = 8,
    strikeForward2 = 9,
    headbutt2 = 10
}

[Serializable]
public class Attack
{
    [SerializeField]
    AttackID id = 0; //true attack identifier
    [SerializeField]
    int damage = 0;
    [SerializeField]
    int energy = 1;
    [SerializeField]
    string animationName = "";

    public AttackID getAttackID () { return id; }
    public int getDamage() { return damage; }
    public int getEnergy() { return energy; }
    public string getAnimationName() { return animationName; }
}

/// <summary>
/// Class that contains all attacks from the Player Character
/// </summary>
[Serializable]
public class Attacks
{
    [SerializeField]
    private Attack[] attacks = new Attack[0];

    /// <summary>
    /// Get the list of all Player Attacks
    /// </summary>
    public ReadOnlyCollection<Attack> GetAttacks
    {
        get
        {
            return Array.AsReadOnly<Attack>(attacks);
        }
    }

    public Attack GetAttackById(AttackID id)
    {
        if ((int)id < attacks.Length)
            return attacks[(int)id];
        else return attacks[0];
    }
}

[Serializable]
public class AttackList
{
    private static Attacks attacks;

    public static Attacks InitializeAttacks ()
    {
        string jsonFile = Resources.Load<TextAsset>("Data/attacks").text;
        attacks = JsonUtility.FromJson<Attacks>(jsonFile);

        int lengthList = attacks.GetAttacks.Count;

        Debug.Log("Character Attacks Loaded: " + lengthList);
        return attacks;
    }
}


