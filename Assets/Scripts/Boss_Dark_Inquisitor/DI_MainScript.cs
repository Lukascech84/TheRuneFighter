using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DI_MainScript : MonoBehaviour
{
    private GameObject MagicProjectilePrefab;
    private GameObject TrapPrefab;
    private GameObject MinionPrefab;
    private GameObject Player;

    private Collider ArenaBounds;

    private Vector3 ProjectileSpawnOffset;
    private float firingRate;
    private bool isShooting;
    private float FireRateTimer = 0f;

    private float TrapSpawnRate;
    private float TrapRateTimer = 0f;
    private bool isTrapping;

    private int MinionsNumber;
    private bool hasMinionsSpawned = false;
    private bool hasAllMinionsDied = true;
    private List<GameObject> activeMinions = new List<GameObject>();

    private int RunesNumber;
    private GameObject RunePrefab;
    private bool hasRunesBeenActive = false;

    private int CurrentPhase;
    private float MaxHealth;
    private float CurrentHealth;

    private DI_BossAttributeManager BossAtm;

    private void Start()
    {
        BossAtm = gameObject.GetComponent<DI_BossAttributeManager>();
        ProjectileSpawnOffset = BossAtm.ProjectileSpawnOffset;
        firingRate = BossAtm.ProjectileFiringRate;
        CurrentPhase = 1;
        ArenaBounds = BossAtm.ArenaBounds;
        MinionsNumber = BossAtm.MinionsNumber;
        MagicProjectilePrefab = BossAtm.MagicProjectilePrefab;
        TrapPrefab = BossAtm.TrapPrefab;
        MinionPrefab = BossAtm.MinionPrefab;
        RunesNumber = BossAtm.RunesNumber;
        RunePrefab = BossAtm.RunesPrefab;
        TrapSpawnRate = BossAtm.TrapSpawnRate;
        Player = BossAtm.Player;
        MaxHealth = BossAtm.Health;
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        CurrentHealth = BossAtm.CurrentHealth;
        CheckMinionStatus();
        HealBoss();

        if (CurrentHealth >= 100 || CurrentPhase == 1)
        {
            isShooting = true;
            isTrapping = true;
            EnterPhase1();
        }
        if (CurrentHealth <= 100 || (CurrentPhase == 1 && CurrentPhase == 2))
        {
            isShooting = false;
            isTrapping = false;
            hasAllMinionsDied = false;
            EnterPhase2();
        }
        if (CurrentHealth <= 20 || (CurrentPhase == 2 && CurrentPhase == 3))
        {
            isShooting = true;
            isTrapping = true;
            EnterPhase3();
        }
    }

    private void EnterPhase1()
    {
        CurrentPhase = 1;

        FireRateTimer += Time.deltaTime;
        TrapRateTimer += Time.deltaTime;

        if (FireRateTimer >= firingRate && isShooting)
        {
            if (!Player) return;
            Fire();
            FireRateTimer = 0f;
        }

        if (TrapRateTimer >= TrapSpawnRate && isTrapping)
        {
            if (!Player) return;
            PlaceTrap();
            TrapRateTimer = 0f;
        }
    }

    private void EnterPhase2()
    {
        if (!hasMinionsSpawned)
        {
            SummonMinions();
            BossAtm.isInvincible = true;
        }
        hasMinionsSpawned = true;

        CurrentPhase = 2;
    }

    private void EnterPhase3()
    {
        CurrentPhase = 3;

        FireRateTimer += Time.deltaTime;

        if (FireRateTimer >= firingRate && isShooting)
        {
            if (!Player) return;
            Fire();
            FireRateTimer = 0f;
        }

        RuneStorm();
    }

    private Vector3 GetRandomPositionInArena(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x), 0, Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private void Fire()
    {
        if (MagicProjectilePrefab)
        {
            transform.LookAt(Player.transform);
            Quaternion adjustedRotation = transform.rotation * Quaternion.Euler(0, 0, 0);
            Vector3 spawnPosition = transform.position + transform.rotation * ProjectileSpawnOffset;

            GameObject spawnedProjectile = Instantiate(MagicProjectilePrefab, spawnPosition, adjustedRotation);

            Vector3 bulletDirection = adjustedRotation * Vector3.forward;

            DI_MagicProjectile projectileScript = spawnedProjectile.GetComponent<DI_MagicProjectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDirection(bulletDirection);
                projectileScript.Spawner = gameObject;
                projectileScript.Player = Player;
            }
        }
    }

    private void PlaceTrap()
    {
        GameObject spawnedTrap = Instantiate(TrapPrefab, Player.transform.position, Quaternion.identity);

        DI_Traps trapScript = spawnedTrap.GetComponent<DI_Traps>();

        if(trapScript != null)
        {
            trapScript.Spawner = gameObject;
            trapScript.Player = Player;
        }
    }

    private void SummonMinions()
    {
        for (int i = 0; i < MinionsNumber; i++) // Pøivolá tøi pøisluhovaèe
        {
            Vector3 spawnPosition = GetRandomPositionInArena(ArenaBounds.bounds);
            GameObject minion = Instantiate(MinionPrefab, spawnPosition, Quaternion.identity);
            activeMinions.Add(minion);
            minion.GetComponent<EnemyAi>().player = Player.transform;
        }
    }

    private void CheckMinionStatus()
    {
        activeMinions.RemoveAll(minion => minion == null);

        if(activeMinions.Count == 0)
        {
            hasAllMinionsDied = true;
            BossAtm.isInvincible = false;
        }
    }

    private void HealBoss()
    {
        if (!hasAllMinionsDied)
        {
            CurrentHealth += 5f * Time.deltaTime; // Léèí se 10 HP za sekundu
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

            BossAtm.CurrentHealth = CurrentHealth;
        }
    }

    private void RuneStorm()
    {
        if (!hasRunesBeenActive)
        {
            hasRunesBeenActive = true;
            for (int i = 0; i < RunesNumber; i++) // 10 run
            {
                Vector3 randomPosition = GetRandomPositionInArena(ArenaBounds.bounds);
                GameObject spawnedRune = Instantiate(RunePrefab, randomPosition, Quaternion.identity);

                DI_Runes runesScript = spawnedRune.GetComponent<DI_Runes>();

                if (runesScript != null)
                {
                    runesScript.Spawner = gameObject;
                }
            }
        }
    }


}
