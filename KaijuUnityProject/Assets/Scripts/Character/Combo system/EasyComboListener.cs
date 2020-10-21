using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class EasyComboListener : MonoBehaviour, IComboMessage
    {
        void Start()
        {

        }

        void Update()
        {

        }

        void IComboMessage.MessageFinishedQuest(Combo combo)
        {
            Debug.Log("------- Combo ran out ------");
            foreach (Attack attack in combo.getAttackChain())
            {
                Debug.Log(attack.getAnimationName());
            }
            Debug.Log("------- ------- ------- -------");
        }
    }
}

