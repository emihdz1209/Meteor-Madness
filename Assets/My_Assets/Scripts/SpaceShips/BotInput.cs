using UnityEngine;

public class BotInput : MonoBehaviour
{
    private Movement movementScript;
    public float botSpeed = 1f;

    void Start()
    {
        movementScript = GetComponent<Movement>();
    }

    void FixedUpdate()
    {
        movementScript.TranslatePlayer(botSpeed);
    }
}