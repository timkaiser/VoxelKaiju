using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using static GameManager;

public class CarSpawnManager : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private List<CarSpawner> spawners = new List<CarSpawner>();
    [SerializeField]
    private List<Car> carsInTraffic = new List<Car>();
    [SerializeField]
    private List<Car> copsInTraffic = new List<Car>();

    public GameObject[] carPrefabs;

    private GameManager manager;

    [Header("Information")]

    /// <summary>
    /// The maximum count of cars allowed on the map.
    /// </summary>
    [SerializeField]
    private int maxCarCount = 10;

    /// <summary>
    /// The maximum amount of police cars allowed on the map.
    /// </summary>
    [SerializeField]
    private int maxCopCount = 5;

    /// <summary>
    /// The pause interval in between car spawns at the same spawner.
    /// </summary>
    private float spawnInterval = 5f;

    /// <summary>
    /// Reference to the Map generator.
    /// </summary>
    private ProceduralGenerator generator;
    [SerializeField]
    private float manhuntTimer = 20f;
    [SerializeField]
    private Coroutine manhuntOngoing;
    #endregion

    #region Basic Functions
    /// <summary>
    /// Initiates the spawn manager.
    /// </summary>
    /// <param name="gen">The Procedural Generator of the current level.</param>
    public void Init(ProceduralGenerator gen, List<CarSpawner> sp)
    {
        generator = gen;
        spawners = sp;
        //Assert.AreNotEqual(spawners.Count, 0, "No spawners found!");
    }

    void Start()
    {
        // Read config data
        //maxCarCount = Config.GetInt(Config.Type.Enemies, "maxCarCount");
        //maxCopCount = Config.GetInt(Config.Type.Enemies, "maxCopCount");
        maxCarCount = generator.width * generator.height;
        maxCopCount = Mathf.CeilToInt(maxCarCount * 0.1f);
        spawnInterval = Config.GetFloat(Config.Type.Enemies, "spawnInterval");
        // Load resources
        carPrefabs = Resources.LoadAll("prefabs/Cars/Original/", typeof(GameObject)).Cast<GameObject>().ToArray();
        //
        manager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!manager.GameState.Equals(GameStates.Paused))
        {
            float currentTime = Time.time;
            int i = 0;
            while (maxCarCount - carsInTraffic.Count - copsInTraffic.Count > 0 && i < spawners.Count)
            {
                CarSpawner spawner = spawners[i];
                if (currentTime - spawner.TimeStamp > spawnInterval)
                {
                    if (maxCopCount - copsInTraffic.Count > 0)
                    {
                        EnterTraffic(spawner.SpawnCar(carPrefabs[0], generator.findNode(spawner.transform.parent.position)));
                    }
                    else
                    {
                        EnterTraffic(spawner.SpawnCar(carPrefabs[1], generator.findNode(spawner.transform.parent.position)));
                    }
                }
                i++;
            }
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.GameStateSwitched.AddListener(SetCarsToSleep);
    }

    private void OnDisable()
    {
        GameManager.Instance.GameStateSwitched.RemoveListener(SetCarsToSleep);
    }
    #endregion

    #region Private Functions
    private IEnumerator manhunt()
    {
        while (manhuntTimer > 0f)
        {
            manhuntTimer -= Time.deltaTime;
            yield return null;
        }
        SetCopsOnDuty(false);
        manhuntOngoing = null;
        manhuntTimer = 20f;
    }
    #endregion

    #region Public Functions

    public void EnterTraffic(Car car)
    {
        if (car.transform.GetComponent<Police>() != null)
            copsInTraffic.Add(car);
        else
            carsInTraffic.Add(car);
    }

    public void LeaveTraffic(Car car)
    {
        if (car.transform.GetComponent<Police>() != null)
            copsInTraffic.Remove(car);
        else
            carsInTraffic.Remove(car);
    }

    public void IncreaseCopCount(int additionalCops)
    {
        maxCopCount = Mathf.Clamp(maxCopCount + additionalCops, 0, Mathf.CeilToInt(maxCarCount * 0.2f));
    }

    public void IncreaseCopCount()
    {
        IncreaseCopCount(Mathf.CeilToInt(maxCopCount * 0.5f));
    }


    public void DecreaseCopCount(int deductingCops)
    {
        maxCopCount = Mathf.Clamp(maxCopCount - deductingCops, 0, maxCarCount);
    }

    public void SetCopsOnDuty(bool duty)
    {
        foreach (Car car in copsInTraffic)
        {
            car.transform.GetComponent<Police>().IsOnDuty = duty;
        }
    }

    public void StartManHunt()
    {
        if (manhuntOngoing != null)
        {
            manhuntTimer = 20f;
        }
        else manhuntOngoing = StartCoroutine(manhunt());
    }

    public void SetCarsToSleep()
    {
        if (manager.GameState.Equals(GameStates.Paused))
        {
            foreach (Car car in carsInTraffic)
            {
                car.Pause();
            }
            foreach (Car car in copsInTraffic)
            {
                car.Pause();
            }
        }
        else if (manager.GameState.Equals(GameStates.Playing))
        {
            foreach (Car car in carsInTraffic)
            {
                car.UnPause();
            }
            foreach (Car car in copsInTraffic)
            {
                car.UnPause();
            }
        }
    }
    #endregion
}
