using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum PointWayEmployee
{
    Reception,
    Office,
    Breakfast,
    Home
}

public class EmployeeController : OfficeController
{
    private const float MIN_DELAY_SECOND = 0f;
    private const float MAX_DELAY_SECOND = 2f;

    private const int MIN_EMPLOYEE_DELAY_SPAWN = 0;
    private const int MAX_EMPLOYEE_DELAY_SPAWN = 2;

    #region ObjectPooler
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
    }

    [SerializeField] private List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void SetPoolObject()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < _officeData.AllEmployee; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }


    private GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't excist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        objectToSpawn.SetActive(true);

        SetDestinationEmployee(objectToSpawn, PointWayEmployee.Reception, _pointReception.position);
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
    #endregion

    #region Points
    [SerializeField] private Transform[] _pointsHome;

    [SerializeField] private Transform _pointReception;
    [SerializeField] private Transform[] _pointOffice;

    [SerializeField] private Transform[] _pointsBreak;
    #endregion

    [SerializeField] private NavMeshData _navMeshData;

    private void Awake()
    {
        NavMesh.AddNavMeshData(_navMeshData);
        SetPoolObject();
    }

    private void OnEnable()
    {
        onWorkDayStarted += DayIsStarted;

        onBreakfast += OnBreakfastDestination;
        onReturnToWork += ReturnToWork;

        onWorkDayFinished += FinishedDay;
    }

    private void Update()
    {

    }

    private void OnBreakfastDestination(GameObject employee)
    {
        StartCoroutine(DestinationEmployeeDelay(employee, PointWayEmployee.Breakfast, 
            _pointsBreak[Random.Range(MIN_EMPLOYEE_DELAY_SPAWN, MAX_EMPLOYEE_DELAY_SPAWN + 1)].position));
    }

    private void ReturnToWork(GameObject employee)
    {
        StartCoroutine(DestinationEmployeeDelay(employee, PointWayEmployee.Office,
            _pointOffice[Random.Range(MIN_EMPLOYEE_DELAY_SPAWN, MAX_EMPLOYEE_DELAY_SPAWN)].position));
    }

    private void SetDestinationEmployee(GameObject employee, PointWayEmployee pointEnum,
        Vector3 point, bool way = true)
    {
        EmployeeData data = employee.GetComponent<EmployeeData>();
        data.SetDestination(pointEnum, point, way);
    }

    private void DayIsStarted ()
    {
        StartCoroutine(SpawnObjectDelay());
    }

    private void FinishedDay(GameObject employee)
    {
        StartCoroutine(DestinationEmployeeDelay(employee, PointWayEmployee.Home,
            _pointsHome[Random.Range(MIN_EMPLOYEE_DELAY_SPAWN, MAX_EMPLOYEE_DELAY_SPAWN)].position));
    }

    private IEnumerator SpawnObjectDelay()
    {
        for (int i = 0; i < _officeData.AllEmployee; i++)
        {
            SpawnFromPool(EMPLOYEES, _pointsHome[Random.Range(
                MIN_EMPLOYEE_DELAY_SPAWN, MAX_EMPLOYEE_DELAY_SPAWN)].position, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(MIN_DELAY_SECOND, MAX_DELAY_SECOND));
        }
    }

    private IEnumerator DestinationEmployeeDelay(GameObject employee,
        PointWayEmployee pointWayEmployee, Vector3 position)
    {
        SetDestinationEmployee(employee, pointWayEmployee, position);
        yield return new WaitForSeconds(Random.Range(MIN_DELAY_SECOND, MAX_DELAY_SECOND));
    }

    private void OnDisable()
    {
        onWorkDayStarted -= DayIsStarted;

        onBreakfast -= OnBreakfastDestination;
        onReturnToWork -= ReturnToWork;
    }
}
