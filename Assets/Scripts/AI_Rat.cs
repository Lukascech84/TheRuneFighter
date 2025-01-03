using UnityEngine;
using UnityEngine.AI;

public class AI_Rat : MonoBehaviour
{
    [Header("Settings")]
    public float sightRange = 10f;
    public float meleeAttackRadius = 2f;
    public float walkPointRange = 5f;
    public float timeBetweenAttacks = 1.5f;

    [Header("References")]
    public LayerMask whatIsGround, whatIsPlayer;

    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;
    private Vector3 walkPoint;
    private bool walkPointSet;
    private bool alreadyAttacked;
    private BaseAttributeManager enemyAttributes;
    private EnemyAttributeManager atm;

    private void Start()
    {
        animator = GetComponent<Animator>();
        atm = GetComponent<EnemyAttributeManager>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("player").transform;

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not attached!");
            enabled = false;
        }
        if (player == null)
        {
            Debug.LogError("Player not found! Ensure there is a GameObject with the tag 'Player'.");
            enabled = false;
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, GetGroundedPlayerPosition());

        if (distanceToPlayer <= sightRange)
        {
            if (distanceToPlayer <= meleeAttackRadius)
            {
                AttackPlayer();
            }
            else
            {
                animator.SetBool("running", true);
                ChasePlayer();
            }
        }
        else
        {
            animator.SetBool("running", true);
            Patrol();
        }
    }

    private void Patrol()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);

            if (Vector3.Distance(transform.position, walkPoint) < 1f)
            {
                walkPointSet = false;
            }
        }
    }

    private void SearchWalkPoint()
    {
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -Vector3.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        // Adjust player's position to be on the ground
        Vector3 targetPosition = GetGroundedPlayerPosition();
        agent.SetDestination(targetPosition);
    }

    private void AttackPlayer()
    {
        // Stop moving and face the player
        agent.SetDestination(transform.position);

        // Always face the player
        Vector3 lookAtPosition = new Vector3(GetGroundedPlayerPosition().x, transform.position.y, GetGroundedPlayerPosition().z);
        transform.LookAt(lookAtPosition);

        if (!alreadyAttacked)
        {
            Attack();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void Attack()
    {
        // Melee útok
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, meleeAttackRadius, whatIsPlayer);

        foreach (Collider playerCollider in hitPlayers)
        {
            enemyAttributes = playerCollider.GetComponent<BaseAttributeManager>();

            if (playerCollider.gameObject == this) return;

            if (enemyAttributes != null)
            {
                if (enemyAttributes.CurrentHealth > 0f)
                {
                    animator.SetBool("running", false);
                    animator.SetTrigger("attacking");

                    atm.DealDamage(enemyAttributes.gameObject, atm.Damage);
                    // Optionálně: Přidat vizuální/akustickou zpětnou vazbu
                }
            }
        }
    }

    private Vector3 GetGroundedPlayerPosition()
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            Vector3 groundedPosition = new Vector3(controller.bounds.center.x, controller.bounds.min.y, controller.bounds.center.z);
            return groundedPosition;
        }
        return player.position;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRadius);

        if (player == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(GetGroundedPlayerPosition(), player.position);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetGroundedPlayerPosition(), 0.2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(player.position, 0.2f);
    }
}