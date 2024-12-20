using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DI_Traps : MonoBehaviour
{
    private float delay;
    private float explosionRadius;
    private float damage;
    private Vector3 SpawnPos;

    private AttributeManager playerAtm;
    private DI_BossAttributeManager bossAtm;
    private AttributeManager hitAtm;

    public GameObject Spawner;
    public GameObject Player;

    void Start()
    {
        bossAtm = Spawner.GetComponent<DI_BossAttributeManager>();
        playerAtm = Player.GetComponent<AttributeManager>();

        delay = bossAtm.TrapDelayWhenSpawned;
        explosionRadius = bossAtm.TrapExplosionRadius;
        damage = bossAtm.TrapDamage;
        SpawnPos = transform.position;

        Invoke("Explode", delay);
    }

    void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(SpawnPos, explosionRadius);
        foreach (Collider enemy in hitColliders)
        {
            hitAtm = enemy.GetComponent<AttributeManager>();
            if (hitAtm != null)
            {
                if (playerAtm.health > 0f)
                {
                    bossAtm.DealDamage(hitAtm.gameObject, damage);
                }
            }
        }
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(SpawnPos, explosionRadius);
    }
}
