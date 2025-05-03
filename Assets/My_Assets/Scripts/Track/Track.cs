using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public List<Point> route = new List<Point>();  // Se puede compartir o definir externamente

    private int indiceActual = 0;
    private Movement movementScript;

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
            float velocidad = movementScript != null ? movementScript.speed : 1f;
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacionDeseada, Time.deltaTime * (velocidad * 2 + 2));
        }

        // Revisar si esta cerca ignorando altura
        if (isClose(transform.position, siguientePunto.position, 5f))
        {
            RevisarCurva(siguientePunto);
            indiceActual++;
        }
    }

    void RevisarCurva(Point punto)
    {
        if (punto.isCurved && movementScript != null)
        {
            float velocidad = movementScript.speed;
            if (velocidad > 0.8f)
                Debug.Log($"{name}: ¡Exceso de velocidad en curva! Vel: {velocidad:F2}");
            else
                Debug.Log($"{name}: Curva tomada correctamente");
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
        // Esto puede moverse a un gestor externo si quieres compartir la ruta
        route.Add(new Point(new Vector3(0, 0, 0), false));
        route.Add(new Point(new Vector3(0, 0, 40), false));
        route.Add(new Point(new Vector3(0, 0, 80), false));
        route.Add(new Point(new Vector3(0, 0, 140), false));
        route.Add(new Point(new Vector3(0, 0, 200), false));
        route.Add(new Point(new Vector3(0, 0, 280), true));
        route.Add(new Point(new Vector3(10, 0, 320), true));
        route.Add(new Point(new Vector3(30, 0, 350), true));
        route.Add(new Point(new Vector3(80, 0, 350), false));
        route.Add(new Point(new Vector3(120, 0, 350), false));
        route.Add(new Point(new Vector3(160, 0, 350), false));
    }

    void VisualizarPuntos()
    {
        foreach (Point punto in route)
        {
            GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubo.transform.position = punto.position;
            cubo.transform.localScale = Vector3.one * 1.5f;

            Renderer renderer = cubo.GetComponent<Renderer>();
            renderer.material.color = punto.isCurved ? Color.red : Color.green;

            cubo.name = "Punto_" + punto.position.ToString();
        }
    }
}
