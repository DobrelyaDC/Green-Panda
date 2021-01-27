public class OfficeData
{
    private const int MIN_EMPLOYEES = 20;

    public static int _allEmployee { get; private set; }
    public static int _inOfficeEmployee { get; private set; }
    public static int _energyEmployee { get; private set; }
    public static int _offsetTime { get; private set; }

    public static bool _isSleepTown { get; private set; }

    public static float worldTime { get; private set; }

    public int AllEmployee
    {
        get
        {
            return _allEmployee;
        }
        set
        {
            _allEmployee = value;
        }
    }

    public int InOfficeEmployee
    {
        get
        {
            return _inOfficeEmployee;
        }
        set
        {
            _inOfficeEmployee = value;
        }
    }

    public int MinEmployees
    {
        get
        {
            return MIN_EMPLOYEES;
        }
    }

    public int EnergyEmployee
    {
        get
        {
            return _energyEmployee;
        }
        set
        {
            _energyEmployee = value;
        }
    }

    public bool IsSleepTown
    {
        get
        {
            return _isSleepTown;
        }
        set
        {
            _isSleepTown = value;
        }
    }

    public int OffsetTime
    {
        get
        {
            return _offsetTime;
        }
        set
        {
            _offsetTime = value;
        }
    }

    public float WorldTime
    {
        get
        {
            return worldTime;
        }
        set
        {
            worldTime = value;
        }
    }
}