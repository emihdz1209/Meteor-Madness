using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    public float moveSpeed= 40;

    public void RotatePlayer(float yAxis)
    {
        transform.Rotate(0, yAxis, 0);
    }

    public void TranslatePlayer(float verticalAxis)
    {
        speed = verticalAxis * moveSpeed * Time.deltaTime + 0.2f;

        transform.Translate(0, 0, speed, Space.Self);
    }
}
