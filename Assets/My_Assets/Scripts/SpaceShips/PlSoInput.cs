using UnityEngine;

public class PlSoInput : MonoBehaviour
{
    private Movement _movementScript;
    public bool enTecho = false; // Estado actual: comienza en el suelo

    public float focusValue;
    public int jawValue;


    void Start()
    {
        _movementScript = GetComponent<Movement>();
    }


    private void Update()
    {
        (float focusV, int jawV) = GetComponent<SocketReceiver>().GetFocusAndJaw();
        Debug.Log($"Focus Index: {focusValue}, Jaw: {jawValue}");
        focusValue = focusV / 100;
        jawValue = jawV / 10;



        if (jawValue == 1)
        {
            Debug.Log("E presionado. Estado actual isFlipping: " + _movementScript.isFlipping);

            if (!_movementScript.isFlipping)
            {
                enTecho = !enTecho;
                _movementScript.FlipPlayer(enTecho);
            }
            else
            {
                Debug.Log("Flip ignorado porque ya está girando.");
            }
        }
    }


    void FixedUpdate()
    {
        _movementScript.TranslatePlayer(focusValue);

    }
}
