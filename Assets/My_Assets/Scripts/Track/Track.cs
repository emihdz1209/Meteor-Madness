using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Track : MonoBehaviour
{
    public List<Point> route = new List<Point>();
    private int currentIndex = 0;
    private Movement movementScript;
    
    // Time tracking
    private float lapStartTime;
    private float[] pointTimes;
    private bool isLapInProgress = false;
    private int lapsCompleted = 0;
    private bool isBot = false;
    
    // Best lap data
    private BestLapData bestLap;
    private string savePath;

    void Start()
    {
        movementScript = GetComponent<Movement>();
        isBot = GetComponent<BotInput>() != null;
        savePath = Path.Combine(Application.dataPath, "Scripts", "Runs", "best_lap.json");
        
        Debug.Log($"Ghost data path: {savePath}");
        
        InitializeRoute();
        VisualizePoints();
        
        if (!isBot)
        {
            InitializeTimeTracking();
            LoadBestLap();
            StartNewLap();
        }
    }

    void LoadBestLap()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string jsonData = File.ReadAllText(savePath);
                bestLap = JsonUtility.FromJson<BestLapData>(jsonData);
                Debug.Log($"Loaded best lap: {bestLap.totalTime:F2}s with {bestLap.segmentTimes.Length} segments");
            }
            else
            {
                bestLap = new BestLapData();
                Debug.Log("No existing ghost data found");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load ghost data: {e.Message}");
            bestLap = new BestLapData();
        }
    }

    void SaveBestLap(float lapTime, float[] segmentTimes)
    {
        if (lapTime < bestLap.totalTime || bestLap.totalTime == 0)
        {
            bestLap.totalTime = lapTime;
            bestLap.segmentTimes = segmentTimes;
            
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                string jsonData = JsonUtility.ToJson(bestLap, true);
                File.WriteAllText(savePath, jsonData);
                
                Debug.Log($"💾 NEW BEST LAP: {lapTime:F2}s");
                Debug.Log($"📁 Saved to: {savePath}");
                Debug.Log($"⏱️ Segment times: {string.Join("s, ", segmentTimes)}s");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save ghost data: {e.Message}");
            }
        }
    }

    void InitializeTimeTracking()
    {
        pointTimes = new float[route.Count];
    }

    void StartNewLap()
    {
        lapStartTime = Time.time;
        isLapInProgress = true;
        currentIndex = 0;
        Debug.Log($"🏁 Lap {lapsCompleted + 1} started");
    }

    void Update()
    {
        NavigatePath();
    }

    void NavigatePath()
    {
        if (route.Count == 0) return;

        if (currentIndex >= route.Count)
        {
            if (!isBot) CompleteLap();
            else currentIndex = 0;
            return;
        }

        Point nextPoint = route[currentIndex];
        Vector3 direction = (nextPoint.position - transform.position).normalized;

        // Horizontal rotation only
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float speed = movementScript != null ? movementScript.speed : 1f;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 
                Time.deltaTime * (speed * 2 + 2));
        }

        if (IsClose(transform.position, nextPoint.position, 5f))
        {
            if (!isBot)
            {
                pointTimes[currentIndex] = Time.time - lapStartTime;
                Debug.Log($"📍 Reached point {currentIndex} at {pointTimes[currentIndex]:F2}s");
            }
            
            CheckCurve(nextPoint);
            currentIndex++;
        }
    }

    void CompleteLap()
    {
        float lapTime = Time.time - lapStartTime;
        lapsCompleted++;
        
        // Calculate segment times
        float[] segmentTimes = new float[pointTimes.Length];
        segmentTimes[0] = pointTimes[0];
        for (int i = 1; i < pointTimes.Length; i++)
        {
            segmentTimes[i] = pointTimes[i] - pointTimes[i-1];
        }

        SaveBestLap(lapTime, segmentTimes);
        
        Debug.Log($"🏁 Lap {lapsCompleted} completed in {lapTime:F2}s");
        Debug.Log($"⏱️ Segment times: {string.Join("s, ", segmentTimes)}s");
        
        StartNewLap();
    }

    void CheckCurve(Point point)
    {
        if (point.isCurved && movementScript != null)
        {
            float speed = movementScript.speed;
            if (speed > 0.8f)
                Debug.Log($"{name}: ⚠️ Too fast in curve! Speed: {speed:F2}");
            else
                Debug.Log($"{name}: ✅ Curve taken correctly");
        }
    }

    bool IsClose(Vector3 a, Vector3 b, float margin)
    {
        Vector2 aXZ = new Vector2(a.x, a.z);
        Vector2 bXZ = new Vector2(b.x, b.z);
        return Vector2.Distance(aXZ, bXZ) < margin;
    }

    void InitializeRoute()
    {
        route.Clear();
        
        // Straight sections
        route.Add(new Point(new Vector3(0, 0, 0), true));
        route.Add(new Point(new Vector3(0, 0, 40), false));
        route.Add(new Point(new Vector3(0, 0, 80), false));
        route.Add(new Point(new Vector3(0, 0, 140), false));
        route.Add(new Point(new Vector3(0, 0, 200), false));
        
        // Curved section
        route.Add(new Point(new Vector3(0, 0, 280), true));
        route.Add(new Point(new Vector3(10, 0, 320), true));
        route.Add(new Point(new Vector3(30, 0, 350), true));
        
        // Final straight
        route.Add(new Point(new Vector3(80, 0, 350), false));
        route.Add(new Point(new Vector3(120, 0, 350), false));
        route.Add(new Point(new Vector3(160, 0, 350), false));
    }

    void VisualizePoints()
    {
        foreach (Point point in route)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.transform.position = point.position;
            marker.transform.localScale = Vector3.one * 1.5f;

            Renderer renderer = marker.GetComponent<Renderer>();
            renderer.material.color = point.isCurved ? Color.red : Color.green;

            marker.name = $"Point_{point.position}";
        }
    }
}