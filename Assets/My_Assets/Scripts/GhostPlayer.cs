using UnityEngine;
using System.Collections.Generic;

public class GhostPlayer : MonoBehaviour
{
    [SerializeField] private float delay = 0f; // No delay, just replay
    private GhostData ghostData;
    private int currentIndex = 0;

    void Start()
    {
        ghostData = GhostRecorder.Instance.LoadRecording();

        if (ghostData == null)
        {
            Debug.Log("No ghost data found. Skipping ghost replay.");
            gameObject.SetActive(false); // Disable ghost if no data
            return;
        }
    }

    void Update()
    {
        if (ghostData == null || ghostData.positions.Count == 0) return;

        // Find the closest time frame
        float targetTime = Time.timeSinceLevelLoad - delay;
        for (int i = currentIndex; i < ghostData.times.Count; i++)
        {
            if (ghostData.times[i] >= targetTime)
            {
                currentIndex = i;
                transform.position = ghostData.positions[i];
                transform.rotation = ghostData.rotations[i];
                break;
            }
        }
    }
}