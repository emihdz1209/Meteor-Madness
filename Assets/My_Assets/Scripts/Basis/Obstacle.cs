using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public GameObject player;
    public float detectionRadius;

    private bool _isPlayerDamaged;

    public float damageTime;
    public float coldDownTime;

    private SkinnedMeshRenderer meshRend;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");

        _isPlayerDamaged = false;

        meshRend = GetComponent<SkinnedMeshRenderer>();
        if(meshRend != null ) { Debug.LogError("Obstacle no tiene Renderer");  }
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance <= detectionRadius && !_isPlayerDamaged)
            {
                Debug.Log("El jugador est� dentro del radio de distancia.");

                player.GetComponent<Movement>().Damage(damageTime);
                StartCoroutine(EnableDamageTimer());

            }

        }
    }

    IEnumerator EnableDamageTimer()
    {
        meshRend.enabled = false;

        _isPlayerDamaged = true;
        yield return new WaitForSeconds(coldDownTime + damageTime);
        _isPlayerDamaged = false;

        meshRend .enabled = true;
    }
    
}
