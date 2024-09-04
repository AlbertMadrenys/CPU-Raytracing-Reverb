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

    public void ForceNextTrigger(bool makeDiffusion)
    {
        m_NextTriggerTimer = 0;
        TriggerRaycast();
        InterpretData(makeDiffusion);
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

    private void InterpretData(bool makeDiffusion = false)
    {
        float eRDelay = 0;
        float eRInten = 0;
        for (int i = 0; i < m_Data.RayDirections.Count; i++)
        {
            eRDelay += m_SignalDataArray[i].EarlyReflectionDelay;
            eRInten += m_SignalDataArray[i].IntensityDifference;
        }

        eRDelay /= m_SignalDataArray.Length;
        eRInten /= m_SignalDataArray.Length;
        Debug.Log(eRInten);
        //m_ReverbFilter.reflectionsDelay = Mathf.Clamp(eRDelay/1000, 0f, 0.3f);
        //m_ReverbFilter.reflectionsLevel = map(eRInten, -10000f, -40f, -10000f, 1000f);//Mathf.Clamp(eRInten * 100, -10000f, 2000f);

        m_ReverbFilter.decayTime = 0; //mapping
        m_ReverbFilter.sendLevel = 0; //mapping
    }

    // Maps a value from ome arbitrary range to another arbitrary range
    public static float map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
    }
}