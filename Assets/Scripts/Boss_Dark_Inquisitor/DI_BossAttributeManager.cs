using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DI_BossAttributeManager : MonoBehaviour
{
    public Collider ArenaBounds;
    public float Health;
    [HideInInspector] public float CurrentHealth;
    [HideInInspector] public bool isInvincible = false;

    public GameObject MagicProjectilePrefab;
    public GameObject MinionPrefab;
    public GameObject TrapPrefab;
    public GameObject Player;

    public float ProjectileLife;
    public float ProjectileSpeed;
    public float ProjectileDamage;
    public Vector3 ProjectileSpawnOffset;
    public float ProjectileFiringRate;

    public float TrapDamage;
    public float TrapExplosionRadius;
    public float TrapDelayWhenSpawned;

    public int MinionsNumber;

    public void TakeDamage(float dmg)
    {
        if (isInvincible) return;
        CurrentHealth -= dmg;

        if (CurrentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void DealDamage(GameObject target, float dmg)
    {
        var atm = target.GetComponent<AttributeManager>();
        if (atm != null)
        {
            atm.TakeDamage(dmg);
        }
    }

    

}
