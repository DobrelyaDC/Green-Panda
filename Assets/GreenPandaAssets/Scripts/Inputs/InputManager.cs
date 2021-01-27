using UnityEngine;

public class InputManager : MonoBehaviour
{
    private const float MIN_BOUNDS_POSITION_X = -70f;
    private const float MAX_BOUNDS_POSITION_X = 81f;
    private const float MIN_BOUNDS_POSITION_Z = -37f;
    private const float MAX_BOUNDS_POSITION_Z = 70f;

    private Camera _camera => Camera.main;

    private Vector3 _newPosition;
    private Vector3 _dragStartPosition;
    private Vector3 _dragCurrentPosition;

    [SerializeField] private float movementTime;

    private void Start()
    {
        _newPosition = transform.position;
    }

    private void Update()
    {
        SetInput();
    }

    private void SetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                _dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                _dragCurrentPosition = ray.GetPoint(entry);

                _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
            }
        }

        transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * movementTime);

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, MIN_BOUNDS_POSITION_X, MAX_BOUNDS_POSITION_X),
            transform.position.y, Mathf.Clamp(transform.position.z, MIN_BOUNDS_POSITION_Z, MAX_BOUNDS_POSITION_Z));
    }
}
