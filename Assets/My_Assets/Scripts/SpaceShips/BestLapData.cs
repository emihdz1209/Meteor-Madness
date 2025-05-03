using UnityEngine;

[System.Serializable]
public class BestLapData
{
    public float totalTime = float.MaxValue;
    public float[] segmentTimes;
}