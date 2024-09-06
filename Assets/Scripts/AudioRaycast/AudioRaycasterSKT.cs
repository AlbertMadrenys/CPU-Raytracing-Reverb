using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NReverbFilter))]
public class AudioRaycasterSKT : MonoBehaviour
{
    [SerializeField] private LayerMask m_CollisionLayerMask;
    [SerializeField] private float m_TriggerInterval = 5;
    [SerializeField] private AudioSourceData m_Data;

    private float m_NextTriggerTimer = 0f;
    private AudioSignalData[] m_SignalDataArray;
    private NReverbFilter m_ReverbFilter;

    private void Awake()
    {
        m_ReverbFilter = GetComponent<NReverbFilter>();
    }

    private void OnEnable()
    {
        m_NextTriggerTimer = m_TriggerInterval;
    }

    void Update()
    {
        m_NextTriggerTimer += Time.deltaTime;
        if(m_NextTriggerTimer >= m_TriggerInterval){
            m_NextTriggerTimer = 0;
            TriggerRaycast();
            InterpretData();
        }
    }

    public void ForceNextTrigger()
    {
        m_NextTriggerTimer = 0;
        TriggerRaycast();
        InterpretData();
    }

    private void TriggerRaycast()
    {
        if(m_SignalDataArray != null) Array.Clear(m_SignalDataArray, 0, m_SignalDataArray.Length);
        m_SignalDataArray = new AudioSignalData[m_Data.RayDirections.Count];

        for(int i = 0; i < m_Data.RayDirections.Count; i++)
        {
            m_SignalDataArray[i] = new AudioSignal().Launch
            (
                new Ray
                (
                    transform.position,
                    transform.InverseTransformDirection(m_Data.RayDirections[i])
                ),
                m_CollisionLayerMask
            );
        }
    }

    private void InterpretData()
    {
        float averageCoeff = 0, mfp = 0;
        float upWeight = 0, downWeight = 0, leftWeight = 0, rightWeight = 0, forwardWeight = 0, backwardWeight = 0;
        float upDist = 0, downDist = 0, leftDist = 0, rightDist = 0, forwardDist = 0, backwardDist = 0;

        for (int i = 0; i < m_Data.RayDirections.Count; i++)
        {
            averageCoeff += m_SignalDataArray[i].AverageAbsorpCoefficient;
            mfp += m_SignalDataArray[i].MeanFreePath;

            CalculateWeight(m_SignalDataArray[i], 0, ref rightWeight, ref rightDist, ref leftWeight, ref leftDist);
            CalculateWeight(m_SignalDataArray[i], 1, ref upWeight, ref upDist, ref downWeight, ref downDist);
            CalculateWeight(m_SignalDataArray[i], 2, ref forwardWeight, ref forwardDist, ref backwardWeight, ref backwardDist);
        }
        averageCoeff /= m_SignalDataArray.Length;
        if (averageCoeff >= 1) averageCoeff = 0.999999f;
        mfp /= m_SignalDataArray.Length;

        rightDist /= rightWeight;
        leftDist /= leftWeight;
        upDist /= upWeight;
        downDist /= downWeight;
        forwardDist /= forwardWeight;
        backwardDist /= backwardWeight;

        var volume = (rightDist + leftDist) * (upDist + downDist) * (forwardDist + backwardDist);
        var reverbField = mfp * (1 - averageCoeff) / (volume*averageCoeff);
        var reverbTime = (-0.161f * mfp) / (4f * Mathf.Log(1 - averageCoeff));

        m_ReverbFilter.sendLevel = Map(reverbField, 1E-06f, 0.08f , 0, 1);
        m_ReverbFilter.decayTime = Mathf.Clamp(reverbTime, 0, 4f);
        
        Debug.DrawRay(transform.position, Vector3.up * upDist, Color.yellow, 2);
        Debug.DrawRay(transform.position, Vector3.down * downDist, Color.yellow, 2);
        Debug.DrawRay(transform.position, Vector3.left * leftDist, Color.yellow, 2);
        Debug.DrawRay(transform.position, Vector3.right * rightDist, Color.yellow, 2);
        Debug.DrawRay(transform.position, Vector3.forward * forwardDist, Color.yellow, 2);
        Debug.DrawRay(transform.position, Vector3.back * backwardDist, Color.yellow, 2);
    }

    // Calculates the weight and distance of the signalData projected to an axis. If the weight is negative, it is stored with a positive sight in the negative weight and distance
    private void CalculateWeight(AudioSignalData signalData, int axis, ref float positiveWeight, ref float positiveDist, ref float negativeWeight, ref float negativeDist)
    {
        Vector3 axisVector = axis == 0 ? Vector3.right : (axis == 1 ? Vector3.up : Vector3.forward);
        float newWeight = Vector3.Project(signalData.InitialDirection, axisVector)[axis];
        if (newWeight > 0f)
        {
            positiveWeight += newWeight;
            positiveDist += newWeight * signalData.SegmentLengths[0];
        }
        else
        {
            negativeWeight -= newWeight;
            negativeDist -= newWeight * signalData.SegmentLengths[0];
        }
    }

    // Maps a value from ome arbitrary range to another arbitrary range
    public static float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
    }
}