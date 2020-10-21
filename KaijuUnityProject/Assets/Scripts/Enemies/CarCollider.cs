using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Management.DestructibleData;

public class CarCollider : MonoBehaviour
{
    [SerializeField]
    float inertiaThresholdToDamage = 1.0f;
    /// <summary>
    /// Handles the car to car collision -> Car will be either damaged or destroyed
    /// </summary>
    /// <param name="collision">The collision taking place between car 1 and car 2.</param>
    void OnCollisionEnter(Collision collision)
    {
        Destruction otherDestruction = collision.transform.GetComponent<Destruction>();
        if (otherDestruction != null)
        {
            if (otherDestruction.type.Equals(DestructibleType.Car))
            {
                Police other = collision.transform.GetComponent<Police>();
                Police self = transform.GetComponent<Police>();
                if (self != null && other == null)
                {
                    otherDestruction.DamageObject(otherDestruction.max_health);
                }
                else
                {
                    otherDestruction.DamageObject(100f * collision.relativeVelocity.magnitude);
                }
                if (collision.relativeVelocity.magnitude > 0.1f)
                {
                    //TODO: this is still played twice
                    AudioSource.PlayClipAtPoint(SoundManager.Instance.GetEffect("CarCrash"), collision.transform.position);
                }
            }
            else if (!otherDestruction.type.Equals(DestructibleType.House) && !otherDestruction.type.Equals(DestructibleType.Landmark))
            {
                otherDestruction.DamageObject(otherDestruction.max_health);
            }
        }
        Character.KaijuController kaijuController = collision.transform.GetComponent<Character.KaijuController>();
        if(kaijuController != null && kaijuController.GetGrowthLevel() < 2)
        {
            Rigidbody rigidbody = transform.GetComponent<Rigidbody>();
            if(rigidbody.inertiaTensor.magnitude > inertiaThresholdToDamage)
            {
                kaijuController.DamageCharacter();
            }
        }
    }
}
