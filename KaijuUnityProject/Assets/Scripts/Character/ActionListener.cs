using Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionListener : MonoBehaviour
{
    public void PerformAction(int id)
    {
        TaskManager.Instance.RegisterPerformedAction((ActionData.ActionType) id );
    }
}
