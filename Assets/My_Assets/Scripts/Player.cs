using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f; // Speed of the player movement

    // Start is called before the first frame update
    void Start()
    {
        // Initialization logic can go here
        Debug.Log("Player initialized");
    }

    void FixedUpdate()
    {
        float moveForward = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime + 0.2f;
        float rotate = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        // Rotate the player around its local Y-axis
        transform.Rotate(0, rotate, 0);

        // Move the player forward or backward relative to its current rotation
        transform.Translate(0, 0, moveForward, Space.Self);
    }
}
