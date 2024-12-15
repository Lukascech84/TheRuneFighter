using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatMelee : MonoBehaviour
{
    //public Animator animator; // Odkaz na animátor postavy
    public Transform attackPoint; // Bod útoku (napø. na konci meèe)
    public float attackRange = 1f; // Dosah útoku
    public LayerMask enemyLayers; // Masková vrstva nepøátel

    private bool canAttack = true; // Kontrola, zda mùže hráè útoèit
    public float attackCooldown = 1f; // Èas mezi útoky

    public AttributeManager playerAtm;
    public AttributeManager enemyAtm;


    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack) StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        canAttack = false; // Zablokuj další útok

        /* Spuštìní animace útoku
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        */

        // Zpoždìní na základì animace (èas na provedení damage)
        yield return new WaitForSeconds(0.2f); // Upravit podle animace

        // Detekce zásahù v dosahu
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            //Debug.Log("Nepøítel zasažen: " + enemy.name);
            // Zpùsob poškození nepøátelùm
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

        // Poèkej na cooldown
        yield return new WaitForSeconds(attackCooldown - 0.2f);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        // Vizualizace dosahu útoku v editoru
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}