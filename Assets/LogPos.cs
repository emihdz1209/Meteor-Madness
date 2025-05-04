using UnityEngine;

public class LogWorldPosOnAwake : MonoBehaviour
{
    void Awake()
    {
        Debug.Log($"{name} world pos = {transform.position}");
    }
}
