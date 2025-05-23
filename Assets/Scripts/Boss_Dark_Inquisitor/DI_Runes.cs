using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DI_Runes : MonoBehaviour
{
    public float activationDelay;
    private float explosionRadius;
    private float runeDamage;

    public GameObject Spawner;
    private DI_BossAttributeManager bossAtm;
    private BaseAttributeManager hitAtm;

    void Start()
    {
        bossAtm = Spawner.GetComponent<DI_BossAttributeManager>();

        explosionRadius = bossAtm.RuneExplosionRadius;
        runeDamage = bossAtm.RuneDamage;

        Invoke("Explode", activationDelay);
    }

    void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider enemy in hitColliders)
        {
            hitAtm = enemy.GetComponent<BaseAttributeManager>();
            if (hitAtm != null && hitAtm.CurrentHealth > 0f)
            {
                bossAtm.DealDamage(hitAtm.gameObject, runeDamage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}