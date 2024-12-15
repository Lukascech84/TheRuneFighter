using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatMelee : MonoBehaviour
{
    //public Animator animator; // Odkaz na anim�tor postavy
    public Transform attackPoint; // Bod �toku (nap�. na konci me�e)
    public float attackRange = 1f; // Dosah �toku
    public LayerMask enemyLayers; // Maskov� vrstva nep��tel

    private bool canAttack = true; // Kontrola, zda m��e hr�� �to�it
    public float attackCooldown = 1f; // �as mezi �toky

    public AttributeManager playerAtm;
    public AttributeManager enemyAtm;


    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack) StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        canAttack = false; // Zablokuj dal�� �tok

        /* Spu�t�n� animace �toku
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        */

        // Zpo�d�n� na z�klad� animace (�as na proveden� damage)
        yield return new WaitForSeconds(0.2f); // Upravit podle animace

        // Detekce z�sah� v dosahu
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            //Debug.Log("Nep��tel zasa�en: " + enemy.name);
            // Zp�sob po�kozen� nep��tel�m
            playerAtm = GetComponent<AttributeManager>();
            enemyAtm = enemy.GetComponent<AttributeManager>();
            if (enemyAtm != null)
            {
                if (enemyAtm.health > 0f)
                {
                    playerAtm.DealDamage(enemyAtm.gameObject, playerAtm.MeleeDamage);
                }
            }
        }

        // Po�kej na cooldown
        yield return new WaitForSeconds(attackCooldown - 0.2f);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        // Vizualizace dosahu �toku v editoru
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}