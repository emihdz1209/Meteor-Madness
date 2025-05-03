using UnityEngine;
using System.Collections.Generic;

public class GhostPlayer : MonoBehaviour
{
    [SerializeField] public float delay = 5f; // 5-second delay
    private List<Vector3> positions;
    private List<Quaternion> rotations;
    private List<float> times;
    private int currentIndex = 0;

    void Start()
    {
        // Get recorded data from player
        var (pos, rot, time) = GhostRecorder.Instance.GetRecordedData();
        positions = pos;
        rotations = rot;
        times = time;
    }

    void Update()
    {
        if (positions == null || positions.Count == 0) return;

        // Find the index where time >= (current time - delay)
        float targetTime = Time.time - delay;
        for (int i = currentIndex; i < times.Count; i++)
        {
            if (times[i] >= targetTime)
            {
                currentIndex = i;
                transform.position = positions[i];
                transform.rotation = rotations[i];
                break;
            }
        }
    }
}