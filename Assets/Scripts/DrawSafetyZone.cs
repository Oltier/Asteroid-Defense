using System.Linq;
using UnityEngine;

public class DrawSafetyZone : MonoBehaviour
{
    [Range(0, 50)] public int Segments = 50;
    private LineRenderer _safetyZone;

    void Start()
    {
        Collider safetyZoneCollider = gameObject.GetComponents<Collider>()
            .OrderByDescending(earthCollider => earthCollider.bounds.extents.x)
            .First();

        float safetyZoneXRadius = safetyZoneCollider.bounds.extents.x;
        float safetyZoneZRadius = safetyZoneCollider.bounds.extents.z;
        
        _safetyZone = gameObject.AddComponent<LineRenderer>();
        _safetyZone.startWidth = 0.1f;
        _safetyZone.endWidth = 0.1f;
        _safetyZone.material = new Material(Shader.Find("Particles/Additive"));
        _safetyZone.startColor = new Color(1f, 0f, 0f, 0.5f);
        _safetyZone.endColor = new Color(1f, 0f, 0f, 0.5f);
        _safetyZone.positionCount = Segments + 1;
        
        CreatePoints(safetyZoneXRadius, safetyZoneZRadius);
        _safetyZone.enabled = true;
    }

    private void CreatePoints(float xRadius, float zRadius)
    {
        float angle = 20f;

        for (int i = 0; i < (Segments + 1); i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * xRadius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * zRadius;

            _safetyZone.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / Segments);
        }
    }
}