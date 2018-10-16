using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Rigidbody RealAsteroid;
    public Rigidbody AsteroidGhost;

    public Rigidbody Earth;
    public GameObject Missile;

    public float MissileSpeed;

    public Text GameOverRestartText;

    private LineRenderer _shotDirectionLineRenderer;
    private Rigidbody _asteroidGhost;

    public void Start()
    {
        _shotDirectionLineRenderer = gameObject.AddComponent<LineRenderer>();
        _shotDirectionLineRenderer.startWidth = 0.1f;
        _shotDirectionLineRenderer.endWidth = 0.1f;
        _shotDirectionLineRenderer.enabled = false;
        GameOverRestartText.text = "";
    }

    private Vector3 _initialPosition = Vector3.zero;
    private Vector3 _currentPosition;

    public void Update()
    {
        if (Earth == null)
        {
            GameOverRestartText.text = "Game over! Press 'R' to restart!";
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                return;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            _initialPosition = GetCurrentMousePosition().GetValueOrDefault();
            if (AsteroidInitialPositionWithinSafeZone(_initialPosition)) return;
            _shotDirectionLineRenderer.SetPosition(0, _initialPosition);
            _shotDirectionLineRenderer.positionCount = 1;
            _shotDirectionLineRenderer.enabled = true;
            _asteroidGhost = Instantiate(AsteroidGhost, _initialPosition, Quaternion.identity);
        }
        else if (Input.GetMouseButton(0) && !AsteroidInitialPositionWithinSafeZone(_initialPosition))
        {
            _currentPosition = GetCurrentMousePosition().GetValueOrDefault();
            Vector3 distance = _currentPosition - _initialPosition;
            _shotDirectionLineRenderer.positionCount = 2;
            _shotDirectionLineRenderer.SetPosition(1, _initialPosition - distance);
        }
        else if (Input.GetMouseButtonUp(0) && !AsteroidInitialPositionWithinSafeZone(_initialPosition))
        {
            _shotDirectionLineRenderer.enabled = false;
            Vector3 releasePosition = GetCurrentMousePosition().GetValueOrDefault();
            Vector3 direction = _initialPosition - releasePosition;
            Rigidbody asteroid = Instantiate(RealAsteroid, _initialPosition, Quaternion.identity);
            asteroid.velocity = direction;
            Destroy(_asteroidGhost.gameObject);

            if (TrajectoryWithinSafetyZone(asteroid.position, asteroid.velocity))
            {
                Rigidbody missileRb = Missile.GetComponent<Rigidbody>();
                Vector3 missileVelocity = CalculateMissileVelocity(asteroid.position, asteroid.velocity);
                float yRotation = -Mathf.Atan2(missileVelocity.z, missileVelocity.x) * (180 / Mathf.PI);
                Vector3 missileRotation = new Vector3(0, yRotation, 0);
                Rigidbody missile = Instantiate(missileRb, Earth.position, Quaternion.Euler(missileRotation));
                missile.velocity = missileVelocity;
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

        Vector3 earthCenter = Earth.position;
        float safetyZoneRadius = safetyZone.bounds.extents.x;

        Vector3 earthToAsteroid = asteroidPosition - earthCenter;

        float a = asteroidVelocity.magnitude;
        float b = 2 * Vector3.Dot(asteroidVelocity, earthToAsteroid);
        float c = earthToAsteroid.magnitude - Mathf.Pow(safetyZoneRadius, 2);
        
        float discriminant = Mathf.Pow(b, 2) - 4 * a * c;

        if (discriminant < 0) return false; // There is no real solution to the equation, so no tangient/intersection
        
        // Solve quadratic equation to get the two intersection point.
        float x1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        float x2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);

        // Return true if either solution is >= 0. Meaning that the Asteroid is moving towards one of the intersection points, or is already on it.
        return x1 >= 0 || x2 >= 0;

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
            //We won't be able to catch the asteroid but try to do our best.
            return asteroidVelocity.normalized * MissileSpeed;
        }

        float shotSpeedOrth = Mathf.Sqrt(MissileSpeed * MissileSpeed - shotVelSpeed * shotVelSpeed);
        Vector3 shotVelOrth = targetDirection * shotSpeedOrth;

        return shotVelOrth + shotVelTang;
    }

    private bool AsteroidInitialPositionWithinSafeZone(Vector3 asteroidInitialPosition)
    {
        Collider safetyZone = Earth.GetComponents<Collider>()
            .OrderByDescending(earthCollider => earthCollider.bounds.extents.x)
            .First();

        float radius = safetyZone.bounds.extents.x;
        Vector3 earthCenter = Earth.position;

        return Vector3.Distance(asteroidInitialPosition, earthCenter) <= radius;
    }
}