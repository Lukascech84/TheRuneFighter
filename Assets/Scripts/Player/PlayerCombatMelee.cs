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
    private Animator Animator;
    private BaseAttributeManager enemyAtm;
    private PlayerAttributeManager PlayerAtm;
    public BoxCollider SwordCollider;
    public GameObject Player;


    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed) isAttacking = true;
        else isAttacking = false;
    }

    void Start()
    {
        Animator = Player.GetComponent<Animator>();
        PlayerAtm = Player.GetComponent<PlayerAttributeManager>();

        Damage = PlayerAtm.MeleeDamage;
        attackCooldown = PlayerAtm.MeleeAttackCooldown;

        SwordCollider.enabled = false;
    }

    private void Update()
    {
        if (PlayerAtm.isDead) return;
        if (isAttacking && canAttack) Attack();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.GetComponent<BaseAttributeManager>() != null)
            {
                enemyAtm = other.GetComponent<BaseAttributeManager>();

                if (other.gameObject == Player) return;

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
        Animator.SetInteger("Attack", Animator.GetInteger("Attack") + 1);
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;

        SwordCollider.enabled = true;

        yield return new WaitForSeconds(attackCooldown);

        SwordCollider.enabled = false;

        Animator.SetInteger("Attack", 0);

        canAttack = true;
    }
}