using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static ProceduralGenerator;
using static TrafficManager;

public class Car : MonoBehaviour
{
    #region Enums
    private enum Carstate { Accelerating, Braking, Stopped, Coasting }
    public enum Turn { Right, Straight, Left, U }
    #endregion

    #region Variables
    /// <summary>
    /// The Node that the car is currently steering towards.
    /// </summary>
    Node currentNode;
    /// <summary>
    /// The node that the car will head towards after the current upcoming node.
    /// </summary>
    Node nextNode;

    [Header("Components")]
    [SerializeField]
    private AudioSource carSource;

    [Header("Info")]
    /// <summary>
    /// The state the car is currently in.
    /// </summary>
    [SerializeField]
    private Carstate state;

    /// <summary>
    /// The current instruction that tells the car what it should do at the current crossing.
    /// </summary>
    [SerializeField]
    private TrafficInstruction instruction = TrafficInstruction.None;

    private Police police;
    /// <summary>
    /// The type of turn that will come up next.
    /// </summary>
    [SerializeField]
    private Turn nextTurn;

    /// <summary>
    /// The currently desired speed that the car is going to drive at.
    /// </summary>
    [SerializeField]
    private float desiredSpeed;

    /// <summary>
    /// The actual current speed of the car (always below maxSpeed)
    /// </summary>
    [SerializeField]
    private float currentSpeed;

    /// <summary>
    /// The sections of a crossing the car will block when turning.
    /// </summary>
    private OccupiedLanes turnLanes;
        
    /// <summary>
    /// The position where the car has to start turning.
    /// </summary>
    private Vector3 turningPos;

    [Header("Parameters")]
    /// <summary>
    /// The maximum speed a car can reach.
    /// </summary>
    [SerializeField]
    private float maxSpeed = 5f;

    /// <summary>
    /// The minimum Speed a car should have, e.g. during curves.
    /// </summary>
    [SerializeField]
    private float turnSpeed = 2.5f;

    /// <summary>
    /// The maximum torque the engine can apply to the car.
    /// </summary>
    [SerializeField]
    private float maxMotorTorque = 30f;

    /// <summary>
    /// The maximum angle the front tires can steer to the side
    /// </summary>
    [SerializeField]
    private float maxSteerAngle = 40f;

    /// <summary>
    /// The desired angle the front tires should be directed to.
    /// </summary>
    private float desiredSteerAngle;

    /// <summary>
    /// The speed how fast the car can change the angle of the front wheels.
    /// </summary>
    [SerializeField]
    private float maxSteerSpeed = 3f;

    /// <summary>
    /// The Rigidbody Center of Mass which will make the car more stable in curves.
    /// </summary>
    [SerializeField]
    private Vector3 centerOfMass;

    /// <summary>
    /// The car's rigidbody.
    /// </summary>
    private Rigidbody body;

    /// <summary>
    /// Reference to the car spawn Manager.
    /// </summary>
    [HideInInspector]
    public CarSpawnManager spawnManager;

    /// <summary>
    /// Reference to the trafficManager.
    /// </summary>
    [HideInInspector]
    public TrafficManager trafficManager;

    /// <summary>
    /// Reference to the game Manager;
    /// </summary>
    private GameManager gameManager;

    /// <summary>
    /// Global distance Factor, contains the distance to either next crossing enter point or car in front.
    /// </summary>
    private float distanceFactor = 0f;

    /// <summary>
    /// The speed limit on the streets.
    /// </summary>
    private float speedLimit = 5f;

    #region Temporary Savestate
    private Vector3 tmpVelocity = Vector3.zero;
    private Vector3 tmpAngleVelocity = Vector3.zero;
    #endregion

    #region Sensors
    private float sensorLength = 4f;
    private Vector3 frontSensorPos = new Vector3(0f, 0.3f, 0.8f);
    #endregion

    #region WheelColliders
    /// <summary>
    /// The Wheel collider of the front left wheel which handles driving.
    /// </summary>
    private WheelCollider wheelFL;
    /// <summary>
    /// The Wheel collider of the front right wheel which handles driving.
    /// </summary>
    private WheelCollider wheelFR;
    /// <summary>
    /// The Wheel collider of the back left wheel which handles driving.
    /// </summary>
    private WheelCollider wheelBL;
    /// <summary>
    /// The Wheel collider of the back right wheel which handles driving.
    /// </summary>
    private WheelCollider wheelBR;
    #endregion

    #endregion

    #region Basic Functions
    private void Awake()
    {
        Transform wheelColliders = transform.Find("WheelColliders");
        wheelFL = wheelColliders.Find("WheelFL").GetComponent<WheelCollider>();
        wheelFR = wheelColliders.Find("WheelFR").GetComponent<WheelCollider>();
        wheelBL = wheelColliders.Find("WheelBL").GetComponent<WheelCollider>();
        wheelBR = wheelColliders.Find("WheelBR").GetComponent<WheelCollider>();
        carSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //get Config values
        ResetSpeedLimit();

        //Set values of components
        body = GetComponent<Rigidbody>();
        body.centerOfMass = centerOfMass;
        police = GetComponent<Police>();
        //Calculate next destination position
        turningPos = nextNode.GetWorldPos();
        desiredSpeed = maxSpeed;
        //
        state = Carstate.Stopped;
        carSource.clip = SoundManager.Instance.GetEffect("CarEngineLoop");
        // Decide the route after the next crossing for the first time [Setup]
        ChooseNextNode();
    }

    /// <summary>
    /// Initiate the car's path values.
    /// </summary>
    /// <param name="current">The current node the car is starting from.</param>
    /// <param name="next">The next node the car will pass.</param>
    public void Init(Node current, Node next)
    {
        currentNode = current;
        nextNode = next;
        GameObject map = GameObject.Find("Map");
        transform.SetParent(map.transform);
        spawnManager = map.GetComponent<CarSpawnManager>();
        trafficManager = map.GetComponent<TrafficManager>();
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!gameManager.GameState.Equals(GameStates.Paused))
        {
            currentSpeed = body.velocity.magnitude * 3.6f;
            Sense();
            Steer();
            Drive();
        }
        else
        {
            StopAccelerating();
            body.Sleep();
        }
    }

    private void Update()
    {
        currentSpeed = body.velocity.magnitude * 3.6f;
        if (!state.Equals(Carstate.Stopped))
        {
            carSource.pitch = currentSpeed / maxSpeed;
        }
    }

    private void OnDestroy()
    {
        if (!instruction.Equals(TrafficInstruction.None)) trafficManager.DeleteCar(this);
        spawnManager.LeaveTraffic(this);
    }
    #endregion

    #region Car Mechanics
    /// <summary>
    /// Check the surroundings of the car with sensors and determine next steps to do.
    /// </summary>
    private void Sense()
    {
        //Initiate Sensor
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPos.z;
        sensorStartPos += transform.up * frontSensorPos.y;
        //Calculate waypoint distance
        float distanceToWaypoint = Vector3.Distance(transform.position, turningPos);
        if (distanceToWaypoint < 0.3f)
        {
            // Decide the new route
            ChooseNextNode();
            distanceToWaypoint = Vector3.Distance(transform.position, turningPos);            
        }
        float distanceToCrossing = GetDistanceToCrossing();
        ReceiveInstructions(distanceToWaypoint);

        RaycastHit hit;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.GetComponentInParent<Car>() != null)
            {
                float distanceToCar = hit.distance;
                float angle = Vector3.Angle(transform.forward, hit.transform.forward);

                if (angle < 45f && distanceToCar < distanceToCrossing)
                {                    
                    float carSpeed = hit.collider.transform.GetComponentInParent<Rigidbody>().velocity.magnitude * 3.6f;
                    if (carSpeed < desiredSpeed) desiredSpeed = carSpeed;
                    else if (!instruction.Equals(TrafficInstruction.Wait))
                    {
                        desiredSpeed = carSpeed;
                    }
                    distanceFactor = 1 - (distanceToCar / sensorLength);
                    return;
                }
            }
            //Debug.DrawLine(sensorStartPos, hit.point);
        }
        DetermineDesiredSpeed(distanceToWaypoint, distanceToCrossing);
    }

    private void ReceiveInstructions(float distanceToWaypoint)
    {
        //Check whether Car has already received instructions
        if (distanceToWaypoint < ProceduralGenerator.section_size && instruction.Equals(TrafficInstruction.None))
        {
            if (police != null && police.IsOnDuty) instruction = TrafficInstruction.Drive;
            else
            {
                turnLanes = GetArrivalLane(GetDirection2DTo(currentNode));
                turnLanes = CalculateTurnLanes(nextTurn, turnLanes);
                trafficManager.EnterCrossing(currentNode, this);
            }
        }
    }

    //distance to waypoint has to be reworked to be the entrance of the crossing
    private void DetermineDesiredSpeed(float distanceToWaypoint, float distanceToCrossing)
    {
        if (police != null && police.IsChasing())
        {
            desiredSpeed = maxSpeed;
        }
        //Depending on the instructions, act.
        else if (instruction.Equals(TrafficInstruction.None) || distanceToWaypoint > ProceduralGenerator.section_size)
        {
            desiredSpeed = speedLimit;
        }
        else if (instruction.Equals(TrafficInstruction.Drive))
        {
            if (nextTurn.Equals(Turn.Straight)) desiredSpeed = speedLimit;
            else
            {
                desiredSpeed = turnSpeed;
                distanceFactor = 1 - (distanceToWaypoint / section_size);
            }
        }
        else //instruction == wait
        {            
            desiredSpeed = 0f;
            distanceFactor = 1 - (distanceToCrossing / section_size);            
        }
    }

    /// <summary>
    /// Steers the car left and right depending on the next waypoint.
    /// </summary>
    private void Steer()
    {
        Vector3 relativeDir = transform.InverseTransformPoint(turningPos);
        desiredSteerAngle = (relativeDir.x  / relativeDir.magnitude) * maxSteerAngle;
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, desiredSteerAngle, Time.deltaTime * maxSteerSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, desiredSteerAngle, Time.deltaTime * maxSteerSpeed);
    }

    /// <summary>
    /// Accelerates or slows down the car depending on the current state.
    /// </summary>
    private void Drive()
    {        
        if (currentSpeed < desiredSpeed)
        {
            if (currentSpeed == 0f)
            {
                carSource.Play();
            }
            Accelerate();
            state = Carstate.Accelerating;
        }
        else if (currentSpeed <= 0.05f)
        {
            state = Carstate.Stopped;
        }
        else if (currentSpeed > desiredSpeed * 1.05f)
        {
            SlowDown();
            state = Carstate.Braking;
        }
        else
        {
            StopAccelerating();
            state = Carstate.Coasting;
        }
    }

    /// <summary>
    /// Accelerate the car by applying motor torque.
    /// </summary>
    private void Accelerate()
    {
        StopBraking();
        float steepnessFactor = 1f;
        if (Mathf.Abs(Vector3.Angle(transform.forward, Vector3.up)) < 85f)
        {
            steepnessFactor = 10f;
            wheelBL.motorTorque = wheelBR.motorTorque = maxMotorTorque * steepnessFactor;
        }
        else
        {
            wheelBL.motorTorque = wheelBR.motorTorque = 0f;
        }
        wheelFL.motorTorque = wheelFR.motorTorque = maxMotorTorque * steepnessFactor;
    }

    /// <summary>
    /// Slow down the car by applying brake torque.
    /// </summary>
    private void SlowDown()
    {
        if (currentSpeed <= 0.05f)
        {
            StopCar();
        }
        else
        {
            StopAccelerating();
            float speedFactor = 1 - (desiredSpeed / currentSpeed);
            Vector3 brakingVelocity = body.velocity * speedFactor * distanceFactor;
            body.velocity -= brakingVelocity * Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Set the brake torque to 0.
    /// </summary>
    private void StopBraking()
    {
        wheelBL.brakeTorque = 0f;
        wheelBR.brakeTorque = 0f;
    }

    /// <summary>
    /// Set the acceleration torque to 0.
    /// </summary>
    private void StopAccelerating()
    {
        wheelFL.motorTorque = 0f;
        wheelFR.motorTorque = 0f;
        wheelBL.motorTorque = 0f;
        wheelBR.motorTorque = 0f;
    }

    private void StopCar()
    {
        state = Carstate.Stopped;
        StopAccelerating();
        desiredSpeed = 0f;
        body.velocity = Vector3.zero;
        carSource.Stop();
    }

    /// <summary>
    /// Adjust the waypoint according to the type of turn.
    /// </summary>
    /// <param name="oldDirection">The vector from the last waypoint to the current waypoint.</param>
    private void AdjustTurningPoint(Vector3 oldDirection)
    {
        Vector3 newDirection = currentNode.getOrientationToNeighbor(nextNode).normalized;
        float turningAngle = Vector3.SignedAngle(oldDirection, newDirection, Vector3.up);
        Vector3 right = Vector3.Cross(Vector3.up, newDirection);

        float laneMidLocation = ProceduralGenerator.street_size / 4;
        // right turn 
        if (turningAngle == 90)
        {
            turningPos += newDirection * laneMidLocation + right * laneMidLocation * 2;
            nextTurn = Turn.Right;
        }
        // straight
        else if (turningAngle == 0)
        {
            turningPos += right * laneMidLocation;
            nextTurn = Turn.Straight;
        }
        // left turn
        else if (turningAngle == -90)
        {
            turningPos += -newDirection * laneMidLocation;
            nextTurn = Turn.Left;
        }
        // u-turn
        else if (turningAngle == 180)
        {
            turningPos += (-right) * laneMidLocation - newDirection * section_size;
            nextTurn = Turn.U;
        }
        //DrawHelper();
    }

    /// <summary>
    /// Determine the next waypoint.
    /// </summary>
    private void ChooseNextNode()
    {
        // Inform the trafficManager
        StartCoroutine(HasLeftCrossing(currentNode));        
        // Change orientation        
        turningPos = nextNode.GetWorldPos();
        // Save the old Direction
        Vector3 oldDirection = currentNode.getOrientationToNeighbor(nextNode).normalized;

        List<Node> neighborNodes = new List<Node>(nextNode.neighbors);
        if (neighborNodes.Count > 1)
            neighborNodes.Remove(currentNode);
        currentNode = nextNode;
        nextNode = neighborNodes[Random.Range(0, neighborNodes.Count)];

        AdjustTurningPoint(oldDirection);
    }

    private IEnumerator HasLeftCrossing(Node node)
    {        
        while (Vector3.Distance(transform.position, node.GetWorldPos()) < section_size / 2)
        {
            yield return null;
        }
        trafficManager.LeaveCrossing(node, this);
        instruction = TrafficInstruction.None;
        turnLanes = 0;
    }

    private float GetDistanceToCrossing()
    {
        float laneMidLocation = ProceduralGenerator.street_size / 4;
        Vector2 direction = GetDirection2DTo(currentNode);
        Vector3 right = Vector3.Cross(Vector3.up, direction);
        Vector3 crossingEnterPoint = currentNode.GetWorldPos() + right * laneMidLocation - new Vector3(direction.x, 0, direction.y) * laneMidLocation * 2;
        return Vector3.Distance(crossingEnterPoint, transform.position);
    }

    private bool IsTurningRight()
    {
        return (wheelFL.steerAngle > 5);
    }

    private bool IsTurningLeft()
    {
        return (wheelFR.steerAngle < -5);
    }

    #endregion

    #region Public Functions
    public Vector2 GetDirection2DTo(Node node)
    {
        Vector3 dir = node.GetWorldPos() - transform.position;
        return Mathf.Abs(dir.x) > Mathf.Abs(dir.z) ? new Vector2(Mathf.Sign(dir.x), 0) : new Vector2( 0, Mathf.Sign(dir.z));
    }

    public void GiveInstructions(TrafficInstruction newInstruction)
    {
        instruction = newInstruction;
    }

    public TrafficInstruction GetInstructions()
    {
        return instruction;
    }

    public OccupiedLanes GetTurnLanes()
    {
        return turnLanes;
    }

    public void OverrideTurnPos(Vector3 godzillaPos)
    {
        turningPos = godzillaPos;
    }

    public void MaxOutSpeed()
    {
        speedLimit = maxSpeed;
    }

    public void ResetSpeedLimit()
    {
        speedLimit = Config.GetFloat(Config.Type.Enemies, "speedLimit");
    }

    public void Pause()
    {
        tmpVelocity = body.velocity;
        tmpAngleVelocity = body.angularVelocity;
        body.isKinematic = true;
        carSource.Stop();
        if (police != null) police.ToggleSiren(false);
    }

    public void UnPause()
    {
        body.velocity = tmpVelocity;
        body.angularVelocity = tmpAngleVelocity;
        body.isKinematic = false;
        carSource.Play();
        if (police != null) police.ToggleSiren(true);
    }
    #endregion

    #region Helper Functions
    private void DrawHelper()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = turningPos;
        DestroyImmediate(sphere.GetComponent<SphereCollider>());
    }
    #endregion
}
