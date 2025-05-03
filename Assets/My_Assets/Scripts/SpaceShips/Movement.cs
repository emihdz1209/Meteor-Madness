using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    public float moveSpeed = 40f;
    [SerializeField] private float rotationSpeed = 100f;

    public void RotatePlayer(float input)
    {
        transform.Rotate(0, input * rotationSpeed * Time.deltaTime, 0);
    }

    public void TranslatePlayer(float input)
    {
        speed = input * moveSpeed * Time.deltaTime;
        transform.Translate(0, 0, speed, Space.Self);
    }
}
