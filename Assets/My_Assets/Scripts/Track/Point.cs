using UnityEngine;

[System.Serializable]
public class Point
{
    public Vector3 position;
    public bool isCurved;

    public Point(Vector3 pos, bool curved)
    {
        position = pos;
        isCurved = curved;
    }
}