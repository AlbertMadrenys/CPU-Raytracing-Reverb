using UnityEngine;

public class AudioObstacle : MonoBehaviour
{
    [SerializeField] private AudioObstacleData m_ObstacleData;

    public AudioObstacleData ObstacleData => m_ObstacleData;
}
