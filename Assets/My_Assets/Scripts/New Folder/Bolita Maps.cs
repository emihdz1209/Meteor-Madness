using UnityEngine;

public class LogPositionOnAwake : MonoBehaviour
{
    void Awake()
    {
        Debug.Log($"{gameObject.name} position: {transform.position}");
    }
}
