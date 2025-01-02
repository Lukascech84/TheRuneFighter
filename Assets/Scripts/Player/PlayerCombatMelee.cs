using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatMelee : MonoBehaviour
{
    private float damage;
    private float attackCooldown;
    private bool canAttack = true;
    private int comboLevel = 0;
    private float comboTimer = 0f;
    private const float ComboMaxTime = 1f; // Maximální èas na pokraèování v kombu

    private Animator animator;
    private BaseAttributeManager enemyAttributes;
    private PlayerAttributeManager playerAttributes;

    public BoxCollider weaponHitbox;
    public GameObject playerObject;

    private void Start()
    {
        animator = playerObject.GetComponent<Animator>();
        playerAttributes = playerObject.GetComponent<PlayerAttributeManager>();

        damage = playerAttributes.MeleeDamage;
        attackCooldown = playerAttributes.MeleeAttackCooldown;

        weaponHitbox.enabled = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (enabled == true)
        {
            if (context.performed && canAttack)
            {
                ExecuteAttack();
            }
        }
    }

    private void Update()
    {
        if (playerAttributes.isDead) return;

        // Správa èasu pro reset komba
        if (comboLevel > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    private void ExecuteAttack()
    {
        comboLevel++;
        comboLevel = Mathf.Clamp(comboLevel, 1, 3); // Limit komba na max. 3 úrovnì

        animator.SetTrigger("Attack");
        animator.SetInteger("AttackLevel", comboLevel);

        comboTimer = ComboMaxTime; // Reset èasovaèe komba

        StartCoroutine(HandleAttackCooldown());
    }

    private IEnumerator HandleAttackCooldown()
    {
        canAttack = false;
        weaponHitbox.enabled = true;

        yield return new WaitForSeconds(attackCooldown);

        weaponHitbox.enabled = false;
        canAttack = true;
    }

    private void ResetCombo()
    {
        comboLevel = 0;
        animator.SetInteger("AttackLevel", 0);
        comboTimer = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BaseAttributeManager>() != null)
        {
            enemyAttributes = other.GetComponent<BaseAttributeManager>();

            if (other.gameObject == playerObject) return;

            if (enemyAttributes != null)
            {
                if (enemyAttributes.CurrentHealth > 0f)
                {
                    playerAttributes.DealDamage(enemyAttributes.gameObject, damage);
                    // Optionálnì: Pøidat vizuální/akustickou zpìtnou vazbu
                }
            }
        }
    }
}
