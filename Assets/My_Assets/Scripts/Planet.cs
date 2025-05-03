using UnityEngine;

public class SlowRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 20f; // degrees per second

    void Update()
    {
        // Rotate around Y axis (Vector3.up)
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
