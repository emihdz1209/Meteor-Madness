using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Movement movementScript;

    void Start()
    {
        movementScript = GetComponent<Movement>();
    }

    void FixedUpdate()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        
        movementScript.TranslatePlayer(vertical);
        movementScript.RotatePlayer(horizontal);
    }
}
