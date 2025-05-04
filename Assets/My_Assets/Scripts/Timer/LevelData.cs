using UnityEngine;

[SerializeField]
public class LevelData {
    public string name;
    public float bestTime;

    public LevelData()
    {
        name = "";
        bestTime = 0;
    }
}
