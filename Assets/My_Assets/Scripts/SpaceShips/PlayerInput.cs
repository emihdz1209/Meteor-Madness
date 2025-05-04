using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Movement _movementScript;
    public bool enTecho = false; // Estado actual: comienza en el suelo

    void Start()
    {
        _movementScript = GetComponent<Movement>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E presionado. Estado actual isFlipping: " + _movementScript.isFlipping);

            if (!_movementScript.isFlipping)
            {
                enTecho = !enTecho;
                _movementScript.FlipPlayer(enTecho);
            }
            else
            {
                Debug.Log("Flip ignorado porque ya está girando.");
            }
        }
    }


    void FixedUpdate()
    {
        _movementScript.TranslatePlayer(Input.GetAxis("Vertical"));

    }
}
