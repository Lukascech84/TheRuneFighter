using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    private Animator Animator;
    private float delay;
    private PlayerAttributeManager playerAtm;
    private LayerMask enemyLayer;
    private TrapAttributeManager trapAtm;
    private bool isPlayerInTrigger = false;

    private void Start()
    {
        trapAtm = GetComponent<TrapAttributeManager>();
        Animator = GetComponent<Animator>();
        delay = trapAtm.delay;
        enemyLayer = trapAtm.enemyLayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerAttributeManager>() != null && ((1 << other.gameObject.layer) & enemyLayer.value) == 0)
        {
            isPlayerInTrigger = true;
            StartCoroutine(DealDamage(other));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerAttributeManager>() != null && ((1 << other.gameObject.layer) & enemyLayer.value) == 0)
        {
            isPlayerInTrigger = false;
        }
    }

    IEnumerator DealDamage(Collider other)
    {
        playerAtm = other.GetComponent<PlayerAttributeManager>();

        if (playerAtm != null && playerAtm.CurrentHealth > 0f)
        {
            yield return new WaitForSeconds(delay);

            Animator.SetTrigger("Attack");

            // Check if the player is still in the trigger
            if (isPlayerInTrigger)
            {
                trapAtm.DealDamage(playerAtm.gameObject, trapAtm.Damage);
            }
        }
    }
}