using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    public List<Point> route = new List<Point>();  // Se puede compartir o definir externamente

    private int indiceActual = 0;
    private Movement movementScript;

    public bool flipPointsAffects;

    public UImanager uimanager;


    void Start()
    {
        movementScript = GetComponent<Movement>();
        StartPointsList();          // Si cada objeto tiene su ruta
        //VisualizarPuntos();         // Opcional: puedes mover esto a un solo objeto si no quieres duplicar
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
        switch (punto.tipo)
        {
            case 1: //Rojo
                float velocidad = movementScript.speed;
                if (velocidad > 0.5f)
                {
                    movementScript.Damage(2);
                    Debug.Log($"{name}: �Exceso de velocidad en curva nooo!");
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
            case 4: //Blanco (win condition true)
                Debug.Log("Race ended");
                uimanager.EndRace();
                indiceActual++; // Move to the next point
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
        route.Add(new Point(new Vector3(-0.12f, -0.263f, 13.987f), 0));
        //route.Add(new Point(new Vector3(0.106f,  0.500f,  0.226f), 0));
        route.Add(new Point(new Vector3(-1.173f, 0.500f, 68.302f), 0));
        route.Add(new Point(new Vector3(-1.575f, 0.500f, 77.245f), 1));
        route.Add(new Point(new Vector3(2.645f,  0.500f, 86.591f), 1));
        route.Add(new Point(new Vector3(9.478f,  0.500f, 91.314f), 1));
        //route.Add(new Point(new Vector3(17.920f, 0.500f, 86.088f), 0));
        route.Add(new Point(new Vector3(24.552f, 0.500f, 78.451f), 1));
        route.Add(new Point(new Vector3(32.290f, 0.500f, 68.302f), 0));
        route.Add(new Point(new Vector3(45.956f, 0.500f, 48.003f), 0));
        route.Add(new Point(new Vector3(56.608f, 0.500f, 33.331f), 0));
        route.Add(new Point(new Vector3(62.436f, 0.500f, 16.751f), 0));
        route.Add(new Point(new Vector3(62.738f, 0.500f,  3.486f), 0));
        route.Add(new Point(new Vector3(60.326f, 0.500f, -8.573f), 0));
        route.Add(new Point(new Vector3(55.503f, 0.500f, -18.421f), 0));
        route.Add(new Point(new Vector3(47.463f, 0.500f, -28.872f), 0));
        route.Add(new Point(new Vector3(38.620f, 0.500f, -38.318f), 0));
        route.Add(new Point(new Vector3(30.179f, 0.500f, -45.553f), 0));
        route.Add(new Point(new Vector3(18.221f, 0.500f, -53.190f), 0));
        route.Add(new Point(new Vector3(5.800f,  0.500f, -54.900f), 1));
        route.Add(new Point(new Vector3(-6.198f, 0.500f, -54.497f), 1));
        route.Add(new Point(new Vector3(-11.926f,0.500f, -49.372f), 1));
        //route.Add(new Point(new Vector3(-9.414f, 0.500f, -37.614f), 0));
        route.Add(new Point(new Vector3(-4.088f, 0.500f, -27.867f), 0));
        route.Add(new Point(new Vector3(-0.068f, 0.500f, -13.698f), 4));

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
                case 4: renderer.material.color = Color.white; break;        // inicio
                default: renderer.material.color = Color.green; break;      // normal
            }

            cubo.name = "Punto_" + punto.position.ToString();
        }
    }

}
