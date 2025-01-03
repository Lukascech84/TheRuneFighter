using UnityEngine;
using UnityEngine.AI;

public class AI_Range : MonoBehaviour
{
    [Header("Settings")]
    public float sightRange = 10f;
    public float attackRange = 5f;
    public float walkPointRange = 5f;
    public float timeBetweenAttacks = 1.5f;
    public float bulletLife = 1.5f;
    public float speed = 6.5f;
    [SerializeField] private float firingRate = 0.04f;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    [Header("References")]
    public LayerMask whatIsGround, whatIsPlayer;
    public GameObject bullet;

    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;
    private Vector3 walkPoint;
    private bool walkPointSet;
    private bool alreadyAttacked;
    private BaseAttributeManager enemyAttributes;
    private EnemyAttributeManager atm;
    private bool playerInSightRange, playerInAttackRange;
    private float timer = 0f;
    private float health;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("player").transform;
        atm = GetComponent<EnemyAttributeManager>();
        if (atm != null) health = atm.BaseHealth;
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

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

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(GetGroundedPlayerPosition());
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        Vector3 lookAtPosition = new Vector3(GetGroundedPlayerPosition().x, transform.position.y, GetGroundedPlayerPosition().z);
        transform.LookAt(lookAtPosition);

        timer += Time.deltaTime;

        if (!alreadyAttacked)
        {
            if (timer >= firingRate)
            {
                Fire();
                timer = 0;
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void Fire()
    {
        if (bullet) // Zkontroluj, zda je přiřazen prefab střely
        {
            // Otočení směru o 90 stupňů kolem osy Y
            Quaternion adjustedRotation = transform.rotation * Quaternion.Euler(0, 0, 0);

            // Vypočítání přesné pozice pro spawn střely s offsetem
            Vector3 spawnPosition = transform.position + transform.rotation * spawnOffset;

            // Vytvoření střely na posunuté pozici s upravenou rotací
            GameObject spawnedBullet = Instantiate(bullet, spawnPosition, adjustedRotation);

            // Vypočítáme nový směr střely
            Vector3 bulletDirection = adjustedRotation * Vector3.forward;

            Bullet_enemy bulletScript = spawnedBullet.GetComponent<Bullet_enemy>();

            // Předáme směr a rychlost střely do komponenty Bullet
            bulletScript.speed = speed;
            bulletScript.bulletLife = bulletLife;
            bulletScript.SetDirection(bulletDirection);

            if (bulletScript != null)
            {
                bulletScript.spawner = gameObject;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        if (player == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(GetGroundedPlayerPosition(), player.position);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetGroundedPlayerPosition(), 0.2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(player.position, 0.2f);
    }
}
