using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public Rigidbody Asteroid;
    
    private LineRenderer _lineRenderer;
    
    public void Start()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.enabled = false;
    }
 
    private Vector3 _initialPosition;
    private Vector3 _currentPosition;
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _initialPosition = GetCurrentMousePosition().GetValueOrDefault();
            _lineRenderer.SetPosition(0, _initialPosition);
            _lineRenderer.positionCount = 1;
            _lineRenderer.enabled = true;
        } 
        else if (Input.GetMouseButton(0))
        {
            _currentPosition = GetCurrentMousePosition().GetValueOrDefault();
            Vector3 distance = _currentPosition - _initialPosition;
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(1, _initialPosition - distance);
        } 
        else if (Input.GetMouseButtonUp(0))
        {
            _lineRenderer.enabled = false;
            Vector3 releasePosition = GetCurrentMousePosition().GetValueOrDefault();
            Vector3 direction = releasePosition - _initialPosition;
            Rigidbody asteroid = Instantiate(Asteroid, _initialPosition, Quaternion.identity);
            asteroid.velocity = -direction;
        }
    }
 
    private Vector3? GetCurrentMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10;
        if (Camera.main == null) return null;
        Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition); 
        return position;
    }
}