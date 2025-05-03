using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public List<Point> route = new List<Point>();
    private int currentIndex = 0;
    private Movement movementScript;
    
    // Time tracking variables
    private float lapStartTime;
    private float[] pointTimes;
    private bool isLapInProgress = false;
    private int lapsCompleted = 0;
    private bool isBot = false;

    void Start()
    {
        movementScript = GetComponent<Movement>();
        isBot = GetComponent<BotInput>() != null;
        
        InitializeRoute();
        VisualizePoints();
        
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
        Debug.Log($"Lap {lapsCompleted + 1} started!");
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
                Debug.Log($"Reached point {currentIndex} at: {pointTimes[currentIndex]:F2} seconds");
            }
            
            CheckCurve(nextPoint);
            currentIndex++;
        }
    }

    void CompleteLap()
    {
        float lapTime = Time.time - lapStartTime;
        lapsCompleted++;
        Debug.Log($"Lap {lapsCompleted} completed in: {lapTime:F2} seconds");
        
        for (int i = 0; i < pointTimes.Length; i++)
        {
            Debug.Log($"Point {i}: {pointTimes[i]:F2}s " + 
                     $"(Segment: {(i > 0 ? pointTimes[i] - pointTimes[i-1] : pointTimes[i]):F2}s)");
        }
        
        StartNewLap();
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
        // Clear existing points
        route.Clear();
        
        // Add points (false = straight, true = curve)
        route.Add(new Point(new Vector3(0, 0, 0), true));
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
            renderer.material.color = point.isCurved ? Color.red : Color.green;

            marker.name = $"Point_{point.position}";
        }
    }
}