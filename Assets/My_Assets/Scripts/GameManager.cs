using UnityEngine;

public class GameManager : MonoBehaviour
{
    void OnApplicationQuit() // Called when stopping play mode
    {
        GhostRecorder.Instance.SaveRecording();
    }

    // Optional: Call this when the race finishes (e.g., crossing finish line)
    public void OnRaceFinish()
    {
        GhostRecorder.Instance.SaveRecording();
    }
}