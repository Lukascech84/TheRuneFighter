using UnityEngine;
using UnityEngine.AI;

public class AI_Ghost : MonoBehaviour
{
    [Header("Settings")]
    public float sightRange = 10f;
    public float meleeAttackRadius = 2f;
    public float dealDamageRadius = 3f;
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
    private bool isAttackGood = false;

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
                AttackPlayer(); // Agent se zastaví zde
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
        Vector3 targetPosition = GetGroundedPlayerPosition();
        agent.SetDestination(targetPosition);
    }

    private void AttackPlayer()
    {
        agent.isStopped = true; // Zastav agenta při útoku
        agent.SetDestination(transform.position); // Zabraň pohybu

        // Otoč agenta směrem k hráči
        Vector3 lookAtPosition = new Vector3(GetGroundedPlayerPosition().x, transform.position.y, GetGroundedPlayerPosition().z);
        transform.LookAt(lookAtPosition);

        if (!alreadyAttacked)
        {
            Attack();
            alreadyAttacked = true;

            // Naplánuj obnovení útoku a pohybu
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void Attack()
    {
        animator.SetBool("running", false);
        animator.SetTrigger("attacking");

        isAttackGood = false;
    }

    private void EnableMovementAnim()
    {
        agent.isStopped = false; // Re-enable movement
    }

    private void DealDamageAnim()
    {
        // Melee attack
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, dealDamageRadius, whatIsPlayer);

        foreach (Collider playerCollider in hitPlayers)
        {
            isAttackGood = false;

            enemyAttributes = playerCollider.GetComponent<BaseAttributeManager>();

            if (playerCollider.gameObject == this) return;

            if (enemyAttributes != null)
            {
                if (enemyAttributes.CurrentHealth > 0f)
                {
                    isAttackGood = true;
                }
            }
        }
        if (isAttackGood)
        {
            atm.DealDamage(enemyAttributes.gameObject, atm.Damage);
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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, dealDamageRadius);

        if (player == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(GetGroundedPlayerPosition(), player.position);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetGroundedPlayerPosition(), 0.2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(player.position, 0.2f);
    }
}