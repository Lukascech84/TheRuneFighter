using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gloryHole : MonoBehaviour
{
    private PlayerAttributeManager playerAtm;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("player");
        playerAtm = player.GetComponent<PlayerAttributeManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            if (playerAtm != null && playerAtm.CurrentHealth > 0f)
            {
                // Zp�soben� maxim�ln�ho po�kozen� k vyvol�n� smrti
                playerAtm.TakeDamage(playerAtm.CurrentHealth);
            }
        }
    }
}