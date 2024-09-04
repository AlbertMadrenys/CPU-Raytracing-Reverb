using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SourceData", menuName = "Audio Raycast/AudioSourceData", order = 1)]
public class AudioSourceData : ScriptableObject
{
    public List<Vector3> RayDirections;
}
