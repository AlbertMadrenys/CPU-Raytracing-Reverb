using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SourceData", menuName = "Audio Raycast/AudioSourceData", order = 1)]
public class AudioSourceData : ScriptableObject
{
    // List of directions. Each element will be a new ray with the origin at the source position and the direction described by the element
    public List<Vector3> RayDirections;
}
