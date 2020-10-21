using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ProceduralGenerator;

public class CarSpawner : MonoBehaviour
{
    /// <summary>
    /// The time when the last car got spawned at this spawner location.
    /// </summary>
    private float timeStamp = 0f;

    /// <summary>
    /// Returns the current timeStamp.
    /// </summary>
    public float TimeStamp
    {
        get { return timeStamp; }
    }

    /// <summary>
    /// Destroys a car when it drives into the collider from the opposite direction.
    /// </summary>
    /// <param name="other">The collider which enters this trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<Car>() != null)
        {
            float angle = Mathf.Abs(Vector3.Angle(other.transform.forward, transform.forward));
            if (angle > 10)
            {
                Destroy(other.gameObject);
            }
        }
    }

    /// <summary>
    /// Spawns a car at the node position of this car Spawner's parent.
    /// </summary>
    /// <param name="carPrefab">A gameobject with attached Car script.</param>
    /// <param name="generator">The proce</param>
    /// <param name="manager"></param>
    public Car SpawnCar(GameObject carPrefab, Node currentNode)
    {
        timeStamp = Time.time;
        Car car = Instantiate(carPrefab, transform.position + transform.right * street_size / 4, transform.rotation).GetComponent<Car>();        
        car.Init(currentNode, currentNode.neighbors[0]);
        return car;
    }
}
