using System.Collections.Generic;
using UnityEngine;

public class AudioSignalData
{
    public int DetectedCollisions;
    public float AccumulatedDistance;
    public List<float> AbsorpCoefficients;
    public List<float> SegmentLengths; // Segment between collisions or between the source and the first collision
    public Vector3 InitialDirection;

    public float EarlyReflectionDelay => AccumulatedDistance / 0.344f;

    public float IntensityDifference // Aproximation of the difference in intensity between direct sound and early reflections
    {
        get
        {
            float i = 0;
            foreach(float l in SegmentLengths) i += -20f * Mathf.Log10(l);
            foreach (float c in AbsorpCoefficients) i += 10f*Mathf.Log10(1-c);
            return i;
        }
    }

    public float MeanFreePath
    {
        get
        {
            float i = 0;
            foreach (float l in SegmentLengths) i += l;
            return i / SegmentLengths.Count;
        }
    }

    public float AverageAbsorpCoefficient
    {
        get
        {
            if(AbsorpCoefficients.Count <= 0) return 1; // If no obstacles, without walls
            float i = 0;
            foreach (float c in AbsorpCoefficients) i += c;
            return i / AbsorpCoefficients.Count;
        }
    }

    public AudioSignalData Initialize(Vector3 direction)
    {
        DetectedCollisions = 0;
        AbsorpCoefficients = new List<float>();
        SegmentLengths = new List<float>();
        InitialDirection = direction.normalized;
        return this;
    }

    public void AddNewHitInfo(RaycastHit hit)
    {
        DetectedCollisions++;
        SegmentLengths.Add(hit.distance);
        AccumulatedDistance += hit.distance;

        if(hit.transform.TryGetComponent(out AudioObstacle obstacle))
        {
            var obstacleData = obstacle.ObstacleMaterial;
            AbsorpCoefficients.Add(obstacleData.AbsorptionCoefficient);
        }
    }

    // Add info about the distance travelled without a collision
    public void AddDistanceWithoutHit(float distance)
    {
        AccumulatedDistance += distance;
        SegmentLengths.Add(distance);
    }
}

public class AudioSignal
{
    private int m_MaxCollisions = 3;
    private float m_MaxDistance = 100f;
    private LayerMask m_CollisionLayerMask;
    private AudioSignalData m_SignalData;

    public AudioSignalData Launch(Ray ray, LayerMask collisionLayerMask)
    {
        m_CollisionLayerMask = collisionLayerMask;
        m_SignalData = new AudioSignalData().Initialize(ray.direction);

        Launch_Internal(ray);
        return m_SignalData; // Return collected data
    }

    private void Launch_Internal(Ray ray)
    {
        float distanceTravelled = m_MaxDistance;
        if (Physics.Raycast(ray, out RaycastHit hit, m_MaxDistance, m_CollisionLayerMask))
        {
            distanceTravelled = hit.distance;
            m_SignalData.AddNewHitInfo(hit);

            if(m_SignalData.DetectedCollisions < m_MaxCollisions)
            {
                Ray nextRay = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
                Launch_Internal(nextRay); // Recursive call with new position and direction
            }
        }
        else
        {
            m_SignalData.AddDistanceWithoutHit(distanceTravelled); // No collision
        }

        Debug.DrawRay(ray.origin, ray.direction * distanceTravelled, Color.red, 2);
    }
}
