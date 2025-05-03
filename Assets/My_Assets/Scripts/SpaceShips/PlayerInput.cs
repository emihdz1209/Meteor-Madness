using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Movement _movementScript;
    [SerializeField] private float steeringSensitivity = 1.5f;

    void Start()
    {
        _movementScript = GetComponent<Movement>();
    }

    void FixedUpdate()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal") * steeringSensitivity;
        
        _movementScript.TranslatePlayer(vertical);
        _movementScript.RotatePlayer(horizontal);
    }
}