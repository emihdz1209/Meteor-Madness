using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class CoordinatesLogger : MonoBehaviour
{
    [Tooltip("Where to dump the .txt.  You’ll find it in your platform’s persistentDataPath.")]
    public string fileName = "points_coordinates.txt";

    void Start()
    {
        var lines = new List<string>();
        // iterate in Hierarchy order
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            Vector3 p = child.position;
            lines.Add($"{child.name}: {p.x:F3}, {p.y:F3}, {p.z:F3}");
        }

        // write out
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllLines(path, lines);
        Debug.Log($"✅ Saved {lines.Count} coordinates to:\n{path}");
    }
}
