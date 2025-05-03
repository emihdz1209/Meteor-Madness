using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Movement _movementScript;
    public float axis;

    void Start()
    {
        _movementScript = GetComponent<Movement>();
    }

    void FixedUpdate()
    {
        axis = Input.GetAxis("Vertical");
        _movementScript.TranslatePlayer(Input.GetAxis("Vertical"));
    }
}