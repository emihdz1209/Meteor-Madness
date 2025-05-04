using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class PlSoInput : MonoBehaviour
{
    private Movement _movementScript;
    public bool enTecho = false; // Estado actual: comienza en el suelo
    public float focusValue;
    public int jawValue;

    public TextMeshProUGUI textMeshPro; // Reference to TextMeshPro component

    void Start()
    {
        _movementScript = GetComponent<Movement>();
    }

    private void Update()
    {
        (float focusV, int jawV) = GetComponent<SocketReceiver>().GetFocusAndJaw();
        Debug.Log($"Focus Index: {focusValue}");
        focusValue = focusV / 100;
        jawValue = jawV / 10;

        // Update the TextMeshPro text with the focusValue
        if (textMeshPro != null)
        {
            textMeshPro.text = $"Focus Value: {focusValue:F2}"; // Format to 2 decimal places
        }
        /* if (jawValue == 1)
        {
            Debug.Log("E presionado. Estado actual isFlipping: " + _movementScript.isFlipping);

            if (!_movementScript.isFlipping)
            {
                enTecho = !enTecho;
                _movementScript.FlipPlayer(enTecho);
            }
            else
            {
                Debug.Log("Flip ignorado porque ya est� girando.");
            }
        } */
    }


    void FixedUpdate()
    {
        _movementScript.TranslatePlayer(focusValue);

    }
}
