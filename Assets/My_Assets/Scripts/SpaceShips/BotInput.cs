using UnityEngine;
using System.IO;

public class BotInput : MonoBehaviour
{
    private Movement _movementScript;
    private BestLapData bestLap;
    private int currentSegment = 0;
    private float segmentStartTime;
    private float[] targetSegmentTimes;
    private bool hasGhostData = false;

    void Start()
    {
        _movementScript = GetComponent<Movement>();
        LoadGhostData();
    }

    void LoadGhostData()
    {
        string path = Path.Combine(Application.dataPath, "Scripts", "Runs", "best_lap.json");
        try
        {
            if (File.Exists(path))
            {
                string jsonData = File.ReadAllText(path);
                bestLap = JsonUtility.FromJson<BestLapData>(jsonData);
                targetSegmentTimes = bestLap.segmentTimes;
                segmentStartTime = Time.time;
                hasGhostData = true;
                
                Debug.Log($"👻 Ghost loaded! Target time: {bestLap.totalTime:F2}s");
                Debug.Log($"🔄 {targetSegmentTimes.Length} segments loaded");
            }
            else
            {
                Debug.Log("No ghost data available - running at default speed");
                _movementScript.TranslatePlayer(1f); // Default speed if no ghost
                enabled = false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load ghost: {e.Message}");
            _movementScript.TranslatePlayer(1f); // Fallback speed
            enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (!hasGhostData || currentSegment >= targetSegmentTimes.Length) return;

        // Calculate progress through current segment (logic to be added here)
    }
}