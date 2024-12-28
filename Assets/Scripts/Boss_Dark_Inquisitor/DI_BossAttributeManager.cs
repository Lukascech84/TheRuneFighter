using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DI_BossAttributeManager : BaseAttributeManager
{
    public Collider ArenaBounds;
    public float PhaseTwoHpPercentage;
    public float PhaseThreeHpPercentage;

    public GameObject MagicProjectilePrefab;
    public GameObject MinionPrefab;
    public GameObject TrapPrefab;
    public GameObject RunesPrefab;
    public GameObject Player;

    public float ProjectileLife;
    public float ProjectileSpeed;
    public float ProjectileDamage;
    public Vector3 ProjectileSpawnOffset;
    public float ProjectileFiringRate;

    public float TrapDamage;
    public float TrapSpawnRate;
    public float TrapExplosionRadius;
    public float TrapDelayWhenSpawned;

    public int MinionsNumber;
    public float HealingPerSecond;

    public float RuneActivationDelay;
    public float RuneExplosionRadius;
    public float RuneDamage;
    public int RunesNumber;
    public float timeBetweenTeleports;
    public float TeleportDistanceFromPlayer;
    public float phase3FireRate;
}
