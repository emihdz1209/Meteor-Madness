using UnityEngine;

public class Player : MonoBehaviour
{
    private StateMachine<Player> stateMachine;


    [SerializeField]
    private float moveSpeed = 5f; // Speed of the player movement

    void Start()
    {
        stateMachine = new StateMachine<Player>();
        stateMachine.ChangeState(new EnemyIdleState(this));

        // Initialization logic can go here
        Debug.Log("Player initialized");
    }

    void Update()
    {
        stateMachine.Update();
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

    public void ChangeColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }

    public void ChangeState(State<Player> newState)
    {
        stateMachine.ChangeState(newState);
    }

    public void ReverseGravity()
    {
        Physics.gravity = -Physics.gravity; // Gravedad invertida

    }
}
