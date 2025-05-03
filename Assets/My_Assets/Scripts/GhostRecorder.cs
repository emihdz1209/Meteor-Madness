using UnityEngine;
using System.Collections.Generic;

public class GhostRecorder : MonoBehaviour
{
    public static GhostRecorder Instance; // Singleton for easy access
    public bool isRecording = true;

    private List<Vector3> recordedPositions = new List<Vector3>();
    private List<Quaternion> recordedRotations = new List<Quaternion>();
    private List<float> recordedTimes = new List<float>();

    void Awake()
    {
        Instance = this; // Set singleton
    }

    void Update()
    {
        if (isRecording)
        {
            recordedPositions.Add(transform.position);
            recordedRotations.Add(transform.rotation);
            recordedTimes.Add(Time.time); // Store exact time
        }
    }

    // Call this to get recorded data
    public (List<Vector3>, List<Quaternion>, List<float>) GetRecordedData()
    {
        return (recordedPositions, recordedRotations, recordedTimes);
    }
}