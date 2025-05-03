using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

[System.Serializable]
public class GhostData
{
    public List<Vector3> positions;
    public List<Quaternion> rotations;
    public List<float> times;
}

public class GhostRecorder : MonoBehaviour
{
    public static GhostRecorder Instance;
    public bool isRecording = true;

    private GhostData recordedData = new GhostData();
    private string savePath;

    void Awake()
    {
        Instance = this;
        // Save to: YOUR_UNITY_PROJECT/Assets/My_Assets/Scripts/runs/ghost_run.json
        savePath = Path.Combine(Application.dataPath, "My_Assets/Scripts/runs/ghost_run.json");
        
        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(savePath));
        
        recordedData.positions = new List<Vector3>();
        recordedData.rotations = new List<Quaternion>();
        recordedData.times = new List<float>();
    }

    void Update()
    {
        if (isRecording)
        {
            recordedData.positions.Add(transform.position);
            recordedData.rotations.Add(transform.rotation);
            recordedData.times.Add(Time.time);
        }
    }

    public void SaveRecording()
    {
        string jsonData = JsonUtility.ToJson(recordedData, true);
        File.WriteAllText(savePath, jsonData);
        Debug.Log($"Ghost race saved to: {savePath}");
    }

    public GhostData LoadRecording()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            return JsonUtility.FromJson<GhostData>(jsonData);
        }
        Debug.Log("No ghost data found. Path checked: " + savePath);
        return null;
    }
}