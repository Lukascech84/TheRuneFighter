using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatMelee : MonoBehaviour
{
    private float Damage;
    private float attackCooldown;
    private bool canAttack = true;
    private bool isAttacking = false;
    private Animator animator;
    private BaseAttributeManager enemyAtm;
    private PlayerAttributeManager PlayerAtm;


    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed) isAttacking = true;
        else isAttacking = false;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        PlayerAtm = GetComponent<PlayerAttributeManager>();

        Damage = PlayerAtm.MeleeDamage;
        attackCooldown = PlayerAtm.MeleeAttackCooldown;
    }

    private void Update()
    {
        if (isAttacking && canAttack) Attack();
    }

    void OnTriggerEnter(Collider other)
    {
            if (other.GetComponent<BaseAttributeManager>() != null)
            {
                enemyAtm = other.GetComponent<BaseAttributeManager>();

                if (other.gameObject == this) return;

                if (enemyAtm != null)
                {
                    if (enemyAtm.CurrentHealth > 0f)
                    {
                        PlayerAtm.DealDamage(enemyAtm.gameObject, Damage);
                    }
                }
            }
    }

    public void Attack()
    {
            animator.SetTrigger("Attack");
            StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        // Zde mùžeš aktivovat Collider pro detekci zásahu
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}