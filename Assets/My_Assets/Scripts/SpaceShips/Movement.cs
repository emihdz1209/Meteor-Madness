using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    public float moveSpeed = 40f;
    public bool  puedeMoverse = true;

    [Header("Smoothing")]
    [Tooltip("How quickly you ramp up to full forward speed")]
    public float accelerationRate = 2f;
    
    // no need to serialize this—it's always 2/3 of your accel
    private float DecelerationRate => accelerationRate * (2f/3f);


    public void RotatePlayer(float yAxis)
    {
        transform.Rotate(0, yAxis, 0);
    }

    public void TranslatePlayer(float verticalAxis)
    {
        if (!puedeMoverse)
        {
            speed = 0f; //0.2f

            return;
        }

        float dt = Time.deltaTime;
        float target = verticalAxis * moveSpeed * dt + 0f; //0.2f

        if (verticalAxis > 0f)
        {
            // accelerate toward positive target
            speed = Mathf.MoveTowards(speed, target, accelerationRate * dt);
        }
        else if (verticalAxis < 0f)
        {
            // decelerate (and reverse) at 2/3rds the accel rate
            speed = Mathf.MoveTowards(speed, target, DecelerationRate * dt);
        }
        else
        {
            // no input → hold the base speed exactly
            speed = 0f; //0.2f
        }

        transform.Translate(0f, 0f, speed, Space.Self);
    }


    public void FlipPlayer(bool haciaTecho)
    {
        if (!isFlipping)
            StartCoroutine(FlipCoroutine(haciaTecho));
    }

    public bool isFlipping = false;

    IEnumerator FlipCoroutine(bool haciaTecho)
    {
        isFlipping = true;

        Quaternion rotacionInicial = transform.rotation;
        Quaternion rotacionFinal = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            haciaTecho ? 180f : 0f // rotacion absoluta en Z
        );

        float duracion = 1.0f;
        float tiempo = 0f;

        Vector3 posInicial = transform.position;
        Vector3 posFinal = posInicial;
        posFinal.y = haciaTecho ? 5f : 0.5f;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;

            transform.rotation = Quaternion.Slerp(rotacionInicial, rotacionFinal, t);
            transform.position = Vector3.Lerp(posInicial, posFinal, t);

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Ajuste final por precisi�n
        transform.rotation = rotacionFinal;
        transform.position = posFinal;

        isFlipping = false;

        Debug.Log($"{name}: Flip {(haciaTecho ? "al techo" : "al suelo")} completado.");
    }




    public void Damage(float time)
    {
        Debug.Log("A llamar ecorrutina");
        StartCoroutine(DamageTime(time));
    }

    IEnumerator DamageTime(float time)
    {
        Debug.Log("Aplicando da�o");

        puedeMoverse = false; //desactiva movimiento
        speed = 0;

        yield return new WaitForSeconds(time);

        puedeMoverse = true; //vuelve a permitir movimiento
        Debug.Log("Movimiento reactivado");
    }


}
