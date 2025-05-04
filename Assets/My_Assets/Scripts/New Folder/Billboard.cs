using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera cameraToFace;    // drag your main Camera here
    private bool isFlipped = false;

    void Update()
    {
        // Flip horizontally on E
        if (Input.GetKeyDown(KeyCode.E))
        {
            isFlipped = !isFlipped;
            Vector3 s = transform.localScale;
            s.x = -s.x;                // invert X scale
            transform.localScale = s;
        }
    }

    void LateUpdate()
    {
        // Always face the camera…
        transform.rotation = cameraToFace.transform.rotation;
    }
}
