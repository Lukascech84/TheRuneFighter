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
            if (playerAtm != null)
            {
                if (playerAtm.CurrentHealth > 0f)
                {
                    playerAtm.CurrentHealth = 0;
                }
            }
        }
    }

}
