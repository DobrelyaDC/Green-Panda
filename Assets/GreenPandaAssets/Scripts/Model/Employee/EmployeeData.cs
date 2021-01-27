using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EmployeeData : MonoBehaviour
{
    private const float MIN_DISTANCE_POINT = 1.5f;

    private PointWayEmployee _pointWayEmployee;
    private StateOffice _stateOffice => OfficeController.stateOffice;

    private NavMeshAgent _agent => GetComponent<NavMeshAgent>();

    private Vector3 _currentDestination;

    private bool inWay = false;
    private bool _breakfast = false;
    private bool _isHome = false;

    private void Awake()
    {

    }

    private void Update()
    {
        CallControlEmployee();
        UpdateBreakfastState();
    }

    public void GetDistanceAndControlEmployee(PointWayEmployee pointEnum, Vector3 point)
    {
        if (Vector3.Distance(_agent.transform.position, _currentDestination) < 
            MIN_DISTANCE_POINT && inWay)
        {
            switch (_pointWayEmployee)
            {
                case PointWayEmployee.Reception:
                    OfficeController.onReturnToWork?.Invoke(gameObject);
                    OfficeController.onOfficeEmployee?.Invoke();
                    break;
                case PointWayEmployee.Office:
                    StopAgent(true);
                    break;
                case PointWayEmployee.Home:
                    gameObject.SetActive(false);
                    break;
            }

            inWay = false;
        }
    }

    private void UpdateBreakfastState()
    {
        if(_stateOffice == StateOffice.Breakfast && !_breakfast && !inWay)
        {
            _pointWayEmployee = PointWayEmployee.Breakfast;
            OfficeController.onBreakfast?.Invoke(gameObject);
            _breakfast = true;
        }

        if(_stateOffice == StateOffice.Work && _breakfast && !inWay)
        {
            _pointWayEmployee = PointWayEmployee.Office;
            OfficeController.onReturnToWork?.Invoke(gameObject);
            _breakfast = false;
        }

        if(_stateOffice == StateOffice.WorkEnd && !_isHome)
        {
            OfficeController.onWorkDayFinished?.Invoke(gameObject);
            _pointWayEmployee = PointWayEmployee.Home;
            _isHome = true;
        }
    }

    public void SetDestination(PointWayEmployee pointEnum, Vector3 point, bool way = true)
    {
        _currentDestination = point;
        _pointWayEmployee = pointEnum;

        _agent.SetDestination(point);
        inWay = way;
    }

    private void CallControlEmployee()
    {
        GetDistanceAndControlEmployee(_pointWayEmployee, _currentDestination);
    }

    private void StopAgent(bool stop)
    {
        _agent.isStopped = stop;
    }

    private void OnDisable()
    {

    }
}
