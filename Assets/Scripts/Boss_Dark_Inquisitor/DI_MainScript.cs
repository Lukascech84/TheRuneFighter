using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DI_MainScript : MonoBehaviour
{
    private GameObject MagicProjectilePrefab;
    private GameObject TrapPrefab;
    private GameObject MinionPrefab;
    private GameObject Player;
    private Animator Animator;

    private Collider ArenaBounds;

    private Vector3 ProjectileSpawnOffset;
    private float firingRate;
    private bool isShooting;
    private float FireRateTimer = 0f;
    private float phase3FireRate;
    private float phase3FireRateTimer = 0f;

    private float TrapSpawnRate;
    private bool isTrapping;
    private float TrapRateTimer = 0f;

    private int MinionsNumber;
    private bool hasMinionsSpawned = false;
    private bool hasAllMinionsDied = true;
    private List<GameObject> activeMinions = new List<GameObject>();

    private int RunesNumber;
    private GameObject RunePrefab;
    private bool hasRunesBeenActive = false;
    private float timeBetweenTeleports;
    private float minTeleportDistance;

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
        MaxHealth = BossAtm.BaseHealth;
        timeBetweenTeleports = BossAtm.timeBetweenTeleports;
        minTeleportDistance = BossAtm.TeleportDistanceFromPlayer;
        phase3FireRate = BossAtm.phase3FireRate;
        Animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        CurrentHealth = BossAtm.CurrentHealth;
        if (!hasAllMinionsDied)
        {
            CheckMinionStatus();
        }
        if (Player != null)
        {
            transform.LookAt(Player.transform);
        }

        HealBoss();

        if (CurrentHealth >= (MaxHealth / 100 * BossAtm.PhaseTwoHpPercentage) && (CurrentPhase == 1 || CurrentPhase == 2))
        {
            isShooting = true;
            isTrapping = true;
            EnterPhase1();
        }
        if (CurrentHealth < (MaxHealth / 100 * BossAtm.PhaseTwoHpPercentage) && (CurrentPhase == 1 || CurrentPhase == 2))
        {
            isShooting = true;
            if (hasAllMinionsDied)
            {
                isTrapping = true;
            }
            else isTrapping = false;

            hasAllMinionsDied = false;
            EnterPhase2();
        }
        if (CurrentHealth <= (MaxHealth / 100 * BossAtm.PhaseThreeHpPercentage) && (CurrentPhase == 2 || CurrentPhase == 3))
        {
            isShooting = true;
            isTrapping = true;
            EnterPhase3();
            if (isShooting)
            {
                phase3FireRateTimer += Time.deltaTime;
                if (phase3FireRateTimer >= phase3FireRate)
                {
                    StartCoroutine(FireBurst(3, 45f)); // Burst støelba ve tøetí fázi
                    phase3FireRateTimer = 0f;
                }
            }
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
            StartCoroutine(Fire());
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

        FireRateTimer += Time.deltaTime;
        TrapRateTimer += Time.deltaTime;

        if (FireRateTimer >= firingRate && isShooting)
        {
            if (!Player) return;
            StartCoroutine(Fire());
            FireRateTimer = 0f;
        }

        if (TrapRateTimer >= TrapSpawnRate && isTrapping)
        {
            if (!Player) return;
            PlaceTrap();
            TrapRateTimer = 0f;
        }

        CurrentPhase = 2;
    }

    private void EnterPhase3()
    {
        if (CurrentPhase != 3)
        {
            CurrentPhase = 3;

            if (!hasRunesBeenActive)
            {
                hasRunesBeenActive = true;
                StartCoroutine(Phase3());
            }
        }
    }

    private IEnumerator Fire()
    {
        if (MagicProjectilePrefab && Player)
        {
            Animator.SetTrigger("attack");
            yield return new WaitForSeconds(0.2f);
            // Spoèítání smìru støely
            Vector3 bulletDirection = (Player.transform.position - transform.position).normalized;

            // Výpoèet pozice pro spawn
            Vector3 spawnPosition = transform.position + transform.forward * ProjectileSpawnOffset.z
                                    + transform.up * ProjectileSpawnOffset.y
                                    + transform.right * ProjectileSpawnOffset.x;

            // Spawn projektilu
            GameObject spawnedProjectile = Instantiate(MagicProjectilePrefab, spawnPosition, Quaternion.identity);

            // Manuální nastavení rotace projektilu
            Quaternion customRotation = Quaternion.LookRotation(bulletDirection) * Quaternion.Euler(90, 0, -90);
            spawnedProjectile.transform.rotation = customRotation;

            // Nastavení smìru støely
            DI_MagicProjectile projectileScript = spawnedProjectile.GetComponent<DI_MagicProjectile>();
            if (projectileScript != null)
            {
                projectileScript.SetDirection(bulletDirection);
                projectileScript.Spawner = gameObject;
                projectileScript.Player = Player;
            }

            Debug.DrawRay(spawnPosition, bulletDirection * 5f, Color.green, 2f);
        }
    }


    private IEnumerator FireBurst(int projectileCount, float spreadAngle)
    {
        if (MagicProjectilePrefab && Player)
        {
            Animator.SetTrigger("attack");
            yield return new WaitForSeconds(0.2f);

            Vector3 baseDirection = (Player.transform.position - transform.position).normalized;
            Vector3 spawnPosition = transform.position + transform.forward * ProjectileSpawnOffset.z
                                    + transform.up * ProjectileSpawnOffset.y
                                    + transform.right * ProjectileSpawnOffset.x;

            for (int i = 0; i < projectileCount; i++)
            {
                float angleOffset = spreadAngle * ((float)i / (projectileCount - 1) - 0.5f);
                Vector3 bulletDirection = Quaternion.Euler(0, angleOffset, 0) * baseDirection;

                GameObject spawnedProjectile = Instantiate(MagicProjectilePrefab, spawnPosition, Quaternion.identity);

                Quaternion customRotation = Quaternion.LookRotation(bulletDirection) * Quaternion.Euler(90, 0, -90);
                spawnedProjectile.transform.rotation = customRotation;

                DI_MagicProjectile projectileScript = spawnedProjectile.GetComponent<DI_MagicProjectile>();
                if (projectileScript != null)
                {
                    projectileScript.SetDirection(bulletDirection);
                    projectileScript.Spawner = gameObject;
                    projectileScript.Player = Player;
                }

                Debug.DrawRay(spawnPosition, bulletDirection * 5f, Color.green, 2f);
            }
        }
    }

    private void PlaceTrap()
    {
        GameObject spawnedTrap = Instantiate(TrapPrefab, Player.transform.position, Quaternion.identity);
        DI_Traps trapScript = spawnedTrap.GetComponent<DI_Traps>();

        if (trapScript != null)
        {
            trapScript.Spawner = gameObject;
            trapScript.Player = Player;
        }
    }

    private void SummonMinions()
    {
        Animator.SetTrigger("mimon");
        for (int i = 0; i < MinionsNumber; i++)
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

        if (activeMinions.Count == 0)
        {
            hasAllMinionsDied = true;
            BossAtm.isInvincible = false;
        }
    }

    private void HealBoss()
    {
        if (!hasAllMinionsDied)
        {
            CurrentHealth += BossAtm.HealingPerSecond * Time.deltaTime;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
            BossAtm.CurrentHealth = CurrentHealth;
        }
    }

    private Vector3 GetRandomPositionInArena(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x), 0, Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private void Teleport()
    {
        Vector3 newPosition;
        do
        {
            newPosition = GetRandomPositionInArena(ArenaBounds.bounds);
        }
        while (Vector3.Distance(transform.position, newPosition) < minTeleportDistance);

        if (Physics.Raycast(newPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f))
        {
            newPosition.y = hit.point.y;
        }
        else
        {
            newPosition.y = ArenaBounds.bounds.min.y;
        }

        transform.position = newPosition;
    }

    private IEnumerator Phase3()
    {
        while (hasRunesBeenActive)
        {
            Teleport();

            for (int i = 0; i < RunesNumber; i++)
            {
                Vector3 randomPosition = GetRandomPositionInArena(ArenaBounds.bounds);
                GameObject spawnedRune = Instantiate(RunePrefab, randomPosition, Quaternion.identity);

                DI_Runes runesScript = spawnedRune.GetComponent<DI_Runes>();
                if (runesScript != null)
                {
                    runesScript.Spawner = gameObject;
                    runesScript.activationDelay = Random.Range(BossAtm.RuneActivationDelay - 2, BossAtm.RuneActivationDelay + 2);
                }
            }

            yield return new WaitForSeconds(timeBetweenTeleports);
        }
    }
}