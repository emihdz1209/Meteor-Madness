using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotInput : MonoBehaviour
{
    private Movement _movementScript;

    public float botSpeed;


    void Start()
    {
        _movementScript = GetComponent<Movement>();
    }

    void FixedUpdate()
    {
        _movementScript.TranslatePlayer(botSpeed);
    }
}
