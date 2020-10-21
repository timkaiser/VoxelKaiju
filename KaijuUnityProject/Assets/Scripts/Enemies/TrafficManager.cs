using System;
using System.Collections.Generic;
using UnityEngine;
using static Car;
using static ProceduralGenerator;

public class TrafficManager : MonoBehaviour
{
    public enum TrafficInstruction { None, Wait, Drive };
    
    /// <summary>
    /// Enum saving the occupied lanes in 4 bits: 0000; each bit can be set to 1 to indicate that the street section is occupied.
    /// -----------
    /// |0001|1000|
    /// |----|----|
    /// |0010|0100|
    /// -----------
    /// </summary>
    [Flags]
    public enum OccupiedLanes
    {
        None = 0,
        LeftTop = 1 << 0,
        LeftBottom = 1 << 1,
        RightBottom = 1 << 2,
        RightTop = 1 << 3                     
    }

    /// <summary>
    /// The dictionary that keeps track of all cars queuing at each street crossing.
    /// </summary>
    private Dictionary<Node, List<Car>> trafficDensity;

    /// <summary>
    /// Initiates the Traffic Manager by setting up the traffic dictionary.
    /// </summary>
    /// <param name="Streetgraph">A list of all nodes that represent the crossings.</param>
    public void Init(List<Node> Streetgraph)
    {
        trafficDensity = new Dictionary<Node, List<Car>>();
        foreach(Node node in Streetgraph)
        {
            trafficDensity.Add(node, new List<Car>());
        }
    }

    /// <summary>
    /// Called by a car to register at a crossing when it enters it.
    /// </summary>
    /// <param name="crossing">Defines the street crossing.</param>
    /// <param name="arrivingCar">The car script of the arriving car.</param>
    public void EnterCrossing(Node crossing, Car arrivingCar)
    {
        List<Car> traffic = trafficDensity[crossing];
        if (!traffic.Contains(arrivingCar))
            traffic.Add(arrivingCar);
        CheckCrossing(traffic);
    }

    /// <summary>
    /// Called by a car to deregister at a crossing when it leaves it.
    /// </summary>
    /// <param name="crossing">Defines the street crossing.</param>
    /// <param name="leavingCar">The car script of the leaving car.</param>
    public void LeaveCrossing(Node crossing, Car leavingCar)
    {
        List<Car> traffic = trafficDensity[crossing];    
        if (traffic.Remove(leavingCar))
        {
            CheckCrossing(traffic);
        }
    }

    /// <summary>
    /// Immediately deletes a car from the list that got destroyed and does not know the node it was subscribed to anymore.
    /// </summary>
    /// <param name="destroyedCar">The car that is being destroyed.</param>
    public void DeleteCar(Car destroyedCar)
    {
        foreach (KeyValuePair<Node, List<Car>> pair in trafficDensity)
        {
            foreach (Car car in pair.Value)
            {
                if (car.Equals(destroyedCar))
                {
                    pair.Value.Remove(destroyedCar);
                    CheckCrossing(pair.Value);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Checks all queuing cars at a crossing and gives orders to drive or wait in first come first serve logic.
    /// </summary>
    /// <param name="traffic">The list that contains all cars queuing for a specific crossing.</param>
    private void CheckCrossing(List<Car> traffic)
    {
        OccupiedLanes streetSections = 0;
        for (int i = 0; i < traffic.Count; i++)
        {
            Car queuingCar = traffic[i];    
            //Checks if necessary lanes for the arriving car are not yet busy
            bool isUnobstructed = (streetSections & queuingCar.GetTurnLanes()) == 0;            
            //If above apply save necessary lanes in streetSections and give instructions to drive
            if (isUnobstructed)
            {
                streetSections |= queuingCar.GetTurnLanes();
                queuingCar.GiveInstructions(TrafficInstruction.Drive);
            }
            //Checks if the turn is the same as the car ahead e.g. for curves which are also regarded as crossings
            //i-1 will always exist because there is some car that is obstructing the crossing
            else if (queuingCar.GetTurnLanes().Equals(traffic[i - 1].GetTurnLanes()) 
                && traffic[i - 1].GetInstructions().Equals(TrafficInstruction.Drive))
            {
                queuingCar.GiveInstructions(TrafficInstruction.Drive);
            }
            else queuingCar.GiveInstructions(TrafficInstruction.Wait);
        }
    }

    /// <summary>
    /// This method takes a normalized directional vector ((0,+1),(+1,0),(0,-1),(-1,0)) and transforms it into an OccupiedTurnLanes enum.
    /// </summary>
    /// <param name="dir">Normalized directional 2D vector along either x or y axis.</param>
    /// <returns>Occupied Turn Lanes enum.</returns>
    public static OccupiedLanes GetArrivalLane(Vector2 dir)
    {
        return ((dir.x == 0) ? (dir.y > 0 ? OccupiedLanes.RightBottom : OccupiedLanes.LeftTop) : (dir.x > 0 ? OccupiedLanes.LeftBottom : OccupiedLanes.RightTop));
    }

    /// <summary>
    /// Calculates the lanes that will be additionally occupied by a turn of a car depending on the arrival lane.
    /// </summary>
    /// <param name="turn">The turn that the car is going to do expressed as Turn enum {right, straight, left, u}</param>
    /// <param name="arrivalLane">The lane of the crossing a car is blocking when it is arriving.</param>
    /// <returns></returns>
    public static OccupiedLanes CalculateTurnLanes(Turn turn, OccupiedLanes arrivalLane)
    {
        //Disregard right turn as no other lane will be occupied
        for (int i = 1; i <= (int)turn; i++)
        {
            //Circular bitshift 0001->0010->0100->1000->0001...
            OccupiedLanes forward = (OccupiedLanes)((int)arrivalLane << 1);
            OccupiedLanes backward = (OccupiedLanes)((int)arrivalLane >> 3);
            //Adds one flag bit to the occupied lanes every iteration progressing counter clockwise
            arrivalLane |= forward | backward; 
        }
        return arrivalLane;
    }
}
