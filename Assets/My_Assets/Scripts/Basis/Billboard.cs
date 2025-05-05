using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera cameraToFace;    // drag your main Camera here
    private bool isFlipped = false;

    private bool signFlipped = false;

    public Player player; // Reference to the Player

    void Awake()
{
    player = GameObject.FindWithTag("Player").GetComponent<Player>();
    if (player == null)
    {
        Debug.LogError("Player not found! Ensure the Player object has the 'Player' tag and a Player component.");
    }
}

    void Update()
    {
        // Flip horizontally on E
        //if (Input.GetKeyDown(KeyCode.E))

        isFlipped = player.GetComponent<PlSoInput>().enTecho;


        if (isFlipped == true && signFlipped == false)
        {
            // Flip the object
            Vector3 localScale = transform.localScale;
            localScale.x *= -1; // Flip the x-axis
            transform.localScale = localScale;

            signFlipped = true; // Set the flag to true
        }
        else if (isFlipped == false && signFlipped == true)
        {
            // Flip back to original
            Vector3 localScale = transform.localScale;
            localScale.x *= -1; // Flip the x-axis back
            transform.localScale = localScale;

            signFlipped = false; // Reset the flag
        }

        //Flip if left click is done
        if (Input.GetMouseButtonDown(0))
        {
            // Flip the object
            Vector3 localScale = transform.localScale;
            localScale.x *= -1; // Flip the x-axis
            transform.localScale = localScale;
        }
    }

    void LateUpdate()
    {
        // Always face the camera…
        transform.rotation = cameraToFace.transform.rotation;
    }
}
