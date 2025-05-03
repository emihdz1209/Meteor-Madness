using UnityEngine;

[System.Serializable]
public class Point: MonoBehaviour
{
    public Vector3 position;
    public bool isCurved;

    public Point(Vector3 pos, bool isCurved)
    {
        this.position = pos;
        this.isCurved = isCurved;
    }
}
