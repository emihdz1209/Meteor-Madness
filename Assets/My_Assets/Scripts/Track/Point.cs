using UnityEngine;

public class Point : MonoBehaviour
{
    public Vector3 position;
    public int tipo; // 0 = normal, 1 = curva, 2 = subir al techo, 3 = bajar del techo

    public Point(Vector3 pos, int tipo)
    {
        this.position = pos;
        this.tipo = tipo;
    }
}
