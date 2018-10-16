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

    private LineRenderer _lineRenderer;
    private Rigidbody _asteroidGhost;

    public void Start()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.enabled = false;
        GameOverRestartText.text = "";
    }

    private Vector3 _initialPosition;
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

            if (TrajectoryWithinSafetyZone(asteroid.position, asteroid.velocity))
            {
                Rigidbody missileRb = Missile.GetComponent<Rigidbody>();
                Vector3 missileVelocity = CalculateMissileVelocity(asteroid.position, asteroid.velocity);
                float yRotation = -Mathf.Atan2(missileVelocity.z, missileVelocity.x) * (180 / Mathf.PI);
                Vector3 missileRotation = new Vector3(0, yRotation, 0);
                Rigidbody missileInstance = Instantiate(missileRb, Vector3.zero, Quaternion.Euler(missileRotation));
                missileInstance.velocity = missileVelocity;
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

        float a = Vector3.Dot(asteroidVelocity, asteroidVelocity);
        float b = 2 * Vector3.Dot(asteroidVelocity, q);
        float c = Vector3.Dot(q, q) - radius * radius;
        float d = b * b - 4 * a * c;

        if (d >= 0)
        {
            float x1 = (-b + Mathf.Sqrt(d)) / (2 * a);
            float x2 = (-b - Mathf.Sqrt(d)) / (2 * a);

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
            //We won't be able to catch the asteroid but try to do our best.
            return asteroidVelocity.normalized * MissileSpeed;
        }

        float shotSpeedOrth = Mathf.Sqrt(MissileSpeed * MissileSpeed - shotVelSpeed * shotVelSpeed);
        Vector3 shotVelOrth = targetDirection * shotSpeedOrth;

        return shotVelOrth + shotVelTang;
    }
}