using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Audio Raycast/AudioObstacleData", order = 1)]
public class AudioObstacleData : ScriptableObject
{
    public float HFAtten;
    public float MFAtten;
    public float LFAtten;
    public float AbsorptionCoefficient;
}
