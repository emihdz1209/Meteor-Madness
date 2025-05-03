using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BestLapData
{
    public float totalTime;
    public float[] segmentTimes;
}

public class Track : MonoBehaviour
{
    public List<Point> route = new List<Point>();
    private int currentIndex = 0;
    private Movement movementScript;

    private float lapStartTime;
    private float[] pointTimes;
    private bool isLapInProgress = false;
    private int lapsCompleted = 0;
    private bool isBot = false;
    private bool hasPassedFirstPoint = false;

    public static float[] bestLapSegmentTimes = null;
    public static float bestLapTime = float.MaxValue;

    void Start()
    {
        movementScript = GetComponent<Movement>();
        isBot = GetComponent<BotInput>() != null;

        if (movementScript == null)
            Debug.LogWarning("Movement script is missing from this GameObject.");

        InitializeRoute();
        VisualizePoints();
        Debug.Log($"Route has {route.Count} points.");

        if (!isBot)
        {
            InitializeTimeTracking();
            StartNewLap();
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
        hasPassedFirstPoint = false;
        Debug.Log($"Lap {lapsCompleted + 1} started!");
    }

    void Update()
    {
        NavigatePath();
    }

    void NavigatePath()
    {
        if (route == null || route.Count == 0) return;

        if (currentIndex < 0 || currentIndex >= route.Count)
        {
            Debug.LogError($"Invalid currentIndex: {currentIndex} for route.Count: {route.Count}");
            return;
        }

        Point nextPoint = route[currentIndex];
        Vector3 direction = (nextPoint.position - transform.position).normalized;

        direction.y = 0;
        if (direction != Vector3.zero)
        {
            float speed = movementScript != null ? movementScript.speed : 1f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * (speed * 2 + 2));
        }

        if (IsClose(transform.position, nextPoint.position, 5f))
        {
            if (!isBot)
            {
                pointTimes[currentIndex] = Time.time - lapStartTime;
                Debug.Log($"Reached point {currentIndex} at: {pointTimes[currentIndex]:F2} seconds");
            }

            CheckCurve(nextPoint);

            if (currentIndex == 0)
            {
                if (hasPassedFirstPoint)
                {
                    if (!isBot) CompleteLap();
                    else currentIndex = 1;
                }
                else
                {
                    hasPassedFirstPoint = true;
                    currentIndex++;
                }
            }
            else
            {
                currentIndex++;
                if (currentIndex >= route.Count)
                {
                    currentIndex = 0;
                }
            }
        }
    }

    void CompleteLap()
    {
        float lapTime = Time.time - lapStartTime;
        lapsCompleted++;

        float[] segmentTimes = new float[route.Count];
        segmentTimes[0] = pointTimes[0];

        for (int i = 1; i < route.Count; i++)
        {
            segmentTimes[i] = pointTimes[i] - pointTimes[i - 1];
        }

        float finalSegmentTime = lapTime - pointTimes[route.Count - 1];
        segmentTimes[0] += finalSegmentTime;

        Debug.Log($"Lap {lapsCompleted} completed in: {lapTime:F2} seconds");
        for (int i = 0; i < segmentTimes.Length; i++)
        {
            Debug.Log($"Segment {i} time: {segmentTimes[i]:F2}s");
        }

        if (lapTime < bestLapTime)
        {
            bestLapTime = lapTime;
            bestLapSegmentTimes = segmentTimes;
            SaveBestLap(lapTime, segmentTimes);
            Debug.Log("New best lap time saved!");
        }

        StartNewLap();
    }

    void SaveBestLap(float lapTime, float[] segmentTimes)
    {
        BestLapData data = new BestLapData
        {
            totalTime = lapTime,
            segmentTimes = segmentTimes
        };

        // Construct the path relative to the Unity project (Assets folder)
        string directoryPath = Application.dataPath + "/My_Assets/Scripts/Runs";
        string filePath = directoryPath + "/best_lap.json";

        // Create the directory if it doesn't exist
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);
        }

        string json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log("Best lap saved to: " + filePath);
    }



    void CheckCurve(Point point)
    {
        if (point.isCurved && movementScript != null)
        {
            float speed = movementScript.speed;
            if (speed > 0.8f)
                Debug.Log($"{name}: Too fast in curve! Speed: {speed:F2}");
            else
                Debug.Log($"{name}: Curve taken correctly");
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

        route.Add(new Point(new Vector3(0, 0, 0), false));
        route.Add(new Point(new Vector3(0, 0, 40), false));
        route.Add(new Point(new Vector3(0, 0, 80), false));
        route.Add(new Point(new Vector3(0, 0, 140), false));
        route.Add(new Point(new Vector3(0, 0, 200), false));
        route.Add(new Point(new Vector3(0, 0, 280), true));
        route.Add(new Point(new Vector3(10, 0, 320), true));
        route.Add(new Point(new Vector3(30, 0, 350), true));
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
            if (route.IndexOf(point) == 0)
                renderer.material.color = Color.blue;
            else
                renderer.material.color = point.isCurved ? Color.red : Color.green;

            marker.name = route.IndexOf(point) == 0 ? "StartFinishLine" : $"Point_{point.position}";
        }
    }
}
