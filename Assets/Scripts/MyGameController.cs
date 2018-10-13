using UnityEngine;

public class MyGameController : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    public void Start()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;
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
            _lineRenderer.positionCount = 2;
//            _lineRenderer.SetPosition(1, Vector3.Dot(Vector3.Dot(_currentPosition, Vector3.back), Vector3.left));
//            Debug.Log(_initialPosition + " " + _currentPosition);
        } 
        else if (Input.GetMouseButtonUp(0))
        {
            _lineRenderer.enabled = false;
            var releasePosition = GetCurrentMousePosition().GetValueOrDefault();
            var direction = releasePosition - _initialPosition;
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