using System;
using System.Linq;
using UnityEditor.Experimental.UIElements;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Rigidbody RealAsteroid;
    public Rigidbody AsteroidGhost;

    public Rigidbody Earth;
    public Rigidbody Missile;

    public float MissileSpeed;

    private LineRenderer _lineRenderer;
    private Rigidbody _asteroidGhost;

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
            _asteroidGhost = Instantiate(AsteroidGhost, _initialPosition, Quaternion.identity);
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
            Rigidbody asteroid = Instantiate(RealAsteroid, _initialPosition, Quaternion.identity);
            asteroid.velocity = -direction;
            Destroy(_asteroidGhost.gameObject);

            if (Earth == null) return;
//                Debug.Log("Trajectory will be in SafetyZone " +
//                          TrajectoryWithinSafetyZone(asteroid.position, asteroid.velocity));
            if (TrajectoryWithinSafetyZone(asteroid.position, asteroid.velocity))
            {
                Rigidbody missile = Instantiate(Missile, Vector3.zero, Quaternion.identity);
                missile.velocity = CalculateMissileVelocity(asteroid.position, asteroid.velocity);
            }
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

    private bool TrajectoryWithinSafetyZone(Vector3 asteroidPosition, Vector3 asteroidVelocity)
    {
        Collider safetyZone = Earth.GetComponents<Collider>()
            .OrderByDescending(earthCollider => earthCollider.bounds.extents.x)
            .First();

        Vector3 earthCenter = safetyZone.bounds.center;
        Vector3 safetyZoneBoundExtent = safetyZone.bounds.extents;
        float radius = safetyZoneBoundExtent.x;

        Vector3 q = asteroidPosition - earthCenter;
        
//        Debug.Log(q);
        
        float a = Vector3.Dot(asteroidVelocity, asteroidVelocity);
        float b = 2 * Vector3.Dot(asteroidVelocity, q);
        float c = Vector3.Dot(q, q) - radius * radius;
        float d = b * b - 4 * a * c;

        if (d >= 0)
        {
            float x1 = (-b + Mathf.Sqrt(d)) / (2 * a);
            float x2 = (-b - Mathf.Sqrt(d)) / (2 * a);
        
//            Debug.Log(x1);
//            Debug.Log(x2);

            return x1 >= 0 || x2 >= 0;
        }

        return false;
    }

    private Vector3 CalculateMissileVelocity(Vector3 asteroidPosition, Vector3 asteroidVelocity)
    {
        Vector3 targetDirection = Vector3.Normalize(asteroidPosition - Earth.position);
        Vector3 targetVelOrth = Vector3.Dot(asteroidVelocity, targetDirection) * targetDirection;

        Vector3 targetVelTang = asteroidVelocity - targetVelOrth;

        Vector3 shotVelTang = targetVelTang;

        float shotVelSpeed = shotVelTang.magnitude;

        if (shotVelSpeed > MissileSpeed)
        {
            return Vector3.zero;
        }

        float shotSpeedOrth = Mathf.Sqrt(MissileSpeed * MissileSpeed - shotVelSpeed * shotVelSpeed);
        Vector3 shotVelOrth = targetDirection * shotSpeedOrth;

        return shotVelOrth + shotVelTang;
    }

}