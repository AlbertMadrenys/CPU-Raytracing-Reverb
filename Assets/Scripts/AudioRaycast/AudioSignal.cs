using System.Collections.Generic;
using UnityEngine;

public class AudioSignalData
{
    public int DetectedCollisions;
    public float AccumulatedDistance;
    public float HFAtten;
    public float MFAtten;
    public float LFAtten;
    public List<float> AbsorbCoeffitients;
    public List<float> PathLengths;

    public float EarlyReflectionDelay => AccumulatedDistance / 0.344f;
    public float IntensityDifference
    {
        get
        {
            float i = 0;
            foreach(float l in PathLengths) i += -20f * Mathf.Log10(l);
            foreach (float c in AbsorbCoeffitients) i += 10f*Mathf.Log10(1-c);
            return i;
        }
    }

    public AudioSignalData Initialize()
    {
        DetectedCollisions = 0;
        HFAtten = 0;
        MFAtten = 0;
        LFAtten = 0;
        AbsorbCoeffitients = new List<float>();
        PathLengths = new List<float>();
        return this;
    }

    public void AddNewHitInfo(RaycastHit hit)
    {
        DetectedCollisions++;
        PathLengths.Add(hit.distance);
        AccumulatedDistance += hit.distance;

        if(hit.transform.TryGetComponent(out AudioObstacle obstacle))
        {
            var obstacleData = obstacle.ObstacleData;
            HFAtten += obstacleData.HFAtten;
            MFAtten += obstacleData.MFAtten;
            LFAtten += obstacleData.LFAtten;
            AbsorbCoeffitients.Add(obstacleData.AbsorptionCoefficient);
        }
    }

    public void AddDistanceWithoutHit(float distance)
    {
        AccumulatedDistance += distance;
        PathLengths.Add(distance);
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
        m_SignalData = new AudioSignalData().Initialize();

        Launch_Internal(ray);
        return m_SignalData;
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
                Launch_Internal(nextRay);
            }
        }
        else
        {
            m_SignalData.AddDistanceWithoutHit(distanceTravelled);
        }

        Debug.DrawRay(ray.origin, ray.direction * distanceTravelled, Color.red, 2);
    }
}
