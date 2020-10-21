using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMotion : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private WheelCollider targetWheel;
    private Vector3 wheelPos = new Vector3();
    private Quaternion wheelRotation = new Quaternion();
    #endregion

    private void Update()
    {
        targetWheel.GetWorldPose(out wheelPos, out wheelRotation);
        transform.position = wheelPos;
        transform.rotation = wheelRotation;
    }
}
