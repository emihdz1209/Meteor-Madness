using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public List<Point> route = new List<Point>();  // Se puede compartir o definir externamente

    private int indiceActual = 0;
    private Movement movementScript;

    public bool flipPointsAffects;

    void Start()
    {
        movementScript = GetComponent<Movement>();
        StartPointsList();          // Si cada objeto tiene su ruta
        VisualizarPuntos();         // Opcional: puedes mover esto a un solo objeto si no quieres duplicar
    }

    void Update()
    {
        PassPoint();
    }

    void PassPoint()
    {
        if (route.Count == 0) return;

        if (indiceActual >= route.Count)
            indiceActual = 0;

        Point siguientePunto = route[indiceActual];
        Vector3 direccion = (siguientePunto.position - transform.position).normalized;

        // Rotacion solo horizontal
        direccion.y = 0;
        if (direccion != Vector3.zero)
        {
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);

            // Mantener X actual (para respetar si esta boca abajo)
            float rotacionX = transform.eulerAngles.x;
            float rotacionZ = transform.eulerAngles.z;

            Quaternion rotacionFinal = Quaternion.Euler(rotacionX, rotacionDeseada.eulerAngles.y, rotacionZ);

            float velocidad = movementScript != null ? movementScript.speed : 1f;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacionFinal, Time.deltaTime * (velocidad * 2 + 2));
        }

        // Revisar si esta cerca ignorando altura
        if (isClose(transform.position, siguientePunto.position, 5f))
        {
            CheckPointAction(siguientePunto);
            indiceActual++;
        }
    }

    void CheckPointAction(Point punto)
    {
        Debug.Log("Tipo de punto: " +  punto.tipo);
        switch (punto.tipo)
        {
            case 1: //Rojo
                float velocidad = movementScript.speed;
                if (velocidad > 0.5f)
                {
                    movementScript.Damage(2);
                    Debug.Log($"{name}: ¡Exceso de velocidad en curva nooo!");
                }
                else
                {
                    Debug.Log($"{name}: Curva tomada correctamente.");
                }
                break;

            case 2: //Azul
                // Subir al techo
                if (flipPointsAffects)
                {
                    movementScript.FlipPlayer(true);
                }
                break;

            case 3: //Amarillo
                // Bajar al suelo
                if (flipPointsAffects)
                {
                    movementScript.FlipPlayer(false);
                }
                break;

            default:
                break;
        }
    }


    bool isClose(Vector3 a, Vector3 b, float margen)
    {
        Vector2 aXZ = new Vector2(a.x, a.z);
        Vector2 bXZ = new Vector2(b.x, b.z);
        return Vector2.Distance(aXZ, bXZ) < margen;
    }


    void StartPointsList()
    {
        route.Add(new Point(new Vector3(0, 0, 0), 0));
        route.Add(new Point(new Vector3(0, 0, 40), 0));
        route.Add(new Point(new Vector3(0, 0, 80), 1)); // curva
        route.Add(new Point(new Vector3(0, 0, 120), 2)); // subir al techo
        route.Add(new Point(new Vector3(0, 5, 160), 0)); // en el techo
        route.Add(new Point(new Vector3(0, 5, 200), 3)); // bajar del techo
        route.Add(new Point(new Vector3(0, 0, 240), 0));
    }

    void VisualizarPuntos()
    {
        foreach (Point punto in route)
        {
            GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubo.transform.position = punto.position;
            cubo.transform.localScale = Vector3.one * 1.5f;

            Renderer renderer = cubo.GetComponent<Renderer>();
            switch (punto.tipo)
            {
                case 1: renderer.material.color = Color.red; break;         // curva
                case 2: renderer.material.color = Color.blue; break;        // subir
                case 3: renderer.material.color = Color.yellow; break;      // bajar
                default: renderer.material.color = Color.green; break;      // normal
            }

            cubo.name = "Punto_" + punto.position.ToString();
        }
    }

}
