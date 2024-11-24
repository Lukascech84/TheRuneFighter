
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    private float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    private AttributeManager atm;
    public GameObject bullet;
    public float bulletLife = 1.5f;
    public float speed = 6.5f;
    [SerializeField] private float firingRate = 0.04f;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero; // Offset pro spawn střely
    private float timer = 0f;

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
        health = atm.health;
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

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
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
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

            // Předáme směr a rychlost střely do komponenty Bullet
            spawnedBullet.GetComponent<Bullet>().speed = speed;
            spawnedBullet.GetComponent<Bullet>().bulletLife = bulletLife;
            spawnedBullet.GetComponent<Bullet>().SetDirection(bulletDirection);
            Bullet bulletScript = spawnedBullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.spawner = gameObject;
            }
        }
    }
}
