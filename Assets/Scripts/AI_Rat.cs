using UnityEngine;
using UnityEngine.AI;

public class AI_Rat : MonoBehaviour
{
    private NavMeshAgent agent; // NavMeshAgent pro pohyb
    private Transform player; // Reference na hráče
    public LayerMask whatIsGround, whatIsPlayer; // Masky pro detekci země a hráče
    private float health; // Zdraví nepřítele

    // Patrolování
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    // Útok
    public float timeBetweenAttacks;
    private bool alreadyAttacked;

    // Detekce a útok
    public float sightRange; // Rozsah vidění
    public float meleeAttackRadius; // Hitbox pro útok

    private EnemyAttributeManager atm; // Správa atributů nepřítele
    private BaseAttributeManager enemyAttributes;

    private void Start()
    {
        atm = GetComponent<EnemyAttributeManager>();
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent není připojen k objektu!");
            enabled = false;
        }

        player = GameObject.FindWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Hráč nebyl nalezen! Ujistěte se, že objekt s tagem 'Player' existuje.");
            enabled = false;
        }

        if (atm != null)
            health = atm.BaseHealth;
    }

    private void Update()
    {
        // Kontrola, zda je hráč v dosahu vidění
        bool playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange)
        {
            Patroling(); // Myš patroluje
        }
        else
        {
            ChaseAndAttack(); // Pronásleduje a útočí na hráče
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Cíl patrolování dosažen
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Výpočet náhodného bodu v dosahu
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChaseAndAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > meleeAttackRadius)
        {
            // Pokud je hráč mimo dosah útoku, pronásleduj
            agent.SetDestination(player.position);
        }
        else
        {
            // Pokud je hráč v dosahu útoku, zastav pohyb a zaútoč
            agent.SetDestination(transform.position);
            if (!alreadyAttacked)
            {
                Attack();
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }

        // Myš se dívá na hráče
        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
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
                    atm.DealDamage(enemyAttributes.gameObject, atm.Damage);
                    // Optionálně: Přidat vizuální/akustickou zpětnou vazbu
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Vizualizace sightRange a meleeAttackRadius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRadius);
    }
}
