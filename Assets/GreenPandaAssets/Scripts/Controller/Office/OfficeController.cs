using UnityEngine;

public enum StateOffice
{
    Work,
    Breakfast,
    WorkEnd
}

public class OfficeController : MonoBehaviour
{
    #region Constant
    private const int OFFSET_TIME_DAY = 5;
    private const int OFFSET_TIME_NIGHT = 20;

    private const int FULL_ENERGY = 100;

    private const int MIN_RANDOM_PAYDAY = 1;
    private const int MAX_RANDOM_PAYDAY = 10;
    private const int DIVISION_VALUE_PAYDAY = 15;

    private const int MIN_PRODUCE_ENERGY = 5;
    private const int MAX_PRODUCE_ENERGY = 15;

    private const int START_DAY_TIME = 7;
    private const int BREAKFAST_TIME = 12;
    private const int BREAKFAST_END_TIME = 14;
    private const int FINISHED_DAY_TIME = 19;
    #endregion

    #region Events
    public delegate void OnWorkDayStarted();
    protected static OnWorkDayStarted onWorkDayStarted;

    public delegate void OnBreakfast(GameObject employee);
    public static OnBreakfast onBreakfast;

    public delegate void OnReturnToWork(GameObject employee);
    public static OnReturnToWork onReturnToWork;

    public delegate void OnWorkDayFinished(GameObject employee);
    public static OnWorkDayFinished onWorkDayFinished;

    public delegate void OnOfficeEmployee();
    public static OnOfficeEmployee onOfficeEmployee;
    #endregion

    protected string EMPLOYEES = "Employee";

    public static StateOffice stateOffice;

    protected OfficeData _officeData => new OfficeData();

    private int _payDay;

    private bool _isPayDay = false;

    private UIController _uiController => GetComponent<UIController>();

    private void Awake()
    {
        onOfficeEmployee += EmployeeComeOffice;

        SetInitilizationEmployees();
        SetStartTime();
    }

    private void Update()
    {
        CalculateTime();
        CheckWorkDay();
    }

    private void CalculateTime()
    {
        _officeData.WorldTime += Time.deltaTime * _officeData.OffsetTime;
    }

    private void SetStartTime()
    {
        // 6 hours
        int hour = 6 * 60;
        _officeData.WorldTime = hour;
        _officeData.OffsetTime = OFFSET_TIME_DAY;
        _officeData.IsSleepTown = true;
    }

    private void SetStartEnergy()
    {
        _officeData.EnergyEmployee = FULL_ENERGY;
    }

    private void PayDayUpdateEveryHour(int hour)
    {
        if (stateOffice == StateOffice.Work && _isPayDay && _officeData.InOfficeEmployee != 0)
        {
            int payHour = _officeData.EnergyEmployee * _officeData.InOfficeEmployee
                * Random.Range(MIN_RANDOM_PAYDAY, MAX_RANDOM_PAYDAY) / DIVISION_VALUE_PAYDAY;

            _payDay += payHour;

            ApplyEnergy(-Random.Range(MIN_PRODUCE_ENERGY, MAX_PRODUCE_ENERGY));
            _uiController.ActiveBonus(true, 0, payHour);

            _isPayDay = false;

            if (hour == BREAKFAST_TIME)
            {
                Breakfast();
            }
        }

        if (stateOffice == StateOffice.Breakfast)
        {
            if (hour == BREAKFAST_END_TIME)
            {
                BreakfastEnd();
            }
        }

        if (hour == FINISHED_DAY_TIME)
        {
            FinishedDay();
        }
    }

    private void Breakfast()
    {
        stateOffice = StateOffice.Breakfast;
    }

    private void BreakfastEnd()
    {
        stateOffice = StateOffice.Work;
    }

    private void FinishedDay()
    {
        stateOffice = StateOffice.WorkEnd;
        _officeData.IsSleepTown = true;
        _officeData.OffsetTime = OFFSET_TIME_NIGHT;
        _officeData.InOfficeEmployee = 0;
    }

    public void ApplyEnergy(int energy)
    {
        _uiController.ActiveBonus(true, 1, energy);
        if (_officeData.EnergyEmployee + energy > 100)
        {
            _officeData.EnergyEmployee = 100;
            return;
        }
        else if (_officeData.EnergyEmployee + energy < 0)
            return;
        _officeData.EnergyEmployee += energy;
    }

    public void CheckWorkDay()
    {
        if (Hours == START_DAY_TIME && _officeData.IsSleepTown)
        {
            onWorkDayStarted?.Invoke();
            stateOffice = StateOffice.Work;
            _officeData.IsSleepTown = false;
            _isPayDay = true;
            _officeData.OffsetTime = OFFSET_TIME_DAY;

            SetStartEnergy();
        }
    }

    protected string FormatTime(float time)
    {
        int hour = (int)time / 60;
        int minutes = (int)time - 60 * hour;
        if (hour == 24) _officeData.WorldTime = 0;

        if (minutes == 59) PayDayUpdateEveryHour(hour);
        else if (minutes == 1) _isPayDay = true;
        return string.Format("{0:00}:{1:00}", hour, minutes);
    }

    private int Hours
    {
        get
        {
            return (int)_officeData.WorldTime / 60;
        }
    }

    private void EmployeeComeOffice()
    {
        _officeData.InOfficeEmployee++;
    }

    private void SetInitilizationEmployees()
    {
        _officeData.AllEmployee = PlayerPrefs.HasKey(EMPLOYEES) ?
        PlayerPrefs.GetInt(EMPLOYEES) : _officeData.MinEmployees;
    }

    private void SaveEmployees()
    {
        if (_officeData.AllEmployee <= PlayerPrefs.GetInt(EMPLOYEES, 0))
            return;
        PlayerPrefs.SetInt(EMPLOYEES, _officeData.AllEmployee);
    }

    private void OnDisable()
    {
        SaveEmployees();

        onOfficeEmployee -= EmployeeComeOffice;
    }
}
