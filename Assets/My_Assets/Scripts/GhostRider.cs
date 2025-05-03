using UnityEngine;

public class GhostRider : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Speed of the ghost kart
    [SerializeField] private float rotationSpeed = 100f; // Turning speed
    [SerializeField] private bool shouldTurnRandomly = true; // Enable random turns
    [SerializeField] private float turnInterval = 3f; // Time between turns (if random)

    private float nextTurnTime;
    private float currentTurnDirection = 0f; // -1 = left, 1 = right, 0 = straight

    void Start()
    {
        nextTurnTime = Time.time + turnInterval;
    }

    void Update()
    {
        // Always move forward
        transform.Translate(0, 0, moveSpeed * Time.deltaTime, Space.Self);

        // Random turning logic (optional)
        if (shouldTurnRandomly && Time.time >= nextTurnTime)
        {
            currentTurnDirection = Random.Range(-1f, 1f); // Random left/right
            nextTurnTime = Time.time + turnInterval;
        }

        // Apply rotation
        transform.Rotate(0, currentTurnDirection * rotationSpeed * Time.deltaTime, 0);
    }
}