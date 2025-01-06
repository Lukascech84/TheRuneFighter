using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Pro práci s NavMesh

public class SpawnEnemies : MonoBehaviour
{
    public GameObject doorToNextLevel;
    public GameObject[] enemyPrefab;
    private Collider ArenaBounds;
    public int numberOfEnemies = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool hasAllMinionsDied;
    public int enemyWaveAddition = 2;
    private bool isFightActive = false;
    public int maxNumberOfWaves = 3;
    private int currentWave = 0;
    private bool doorHasOpened = false;

    private void Start()
    {
        ArenaBounds = GetComponent<Collider>();
    }

    private void Update()
    {
        if (isFightActive)
        {
            CheckMinionStatus();
            if (hasAllMinionsDied && currentWave <= maxNumberOfWaves) SummonMinions();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("player"))
        {
            isFightActive = true;
        }
    }

    private void SummonMinions()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPosition(ArenaBounds.bounds);
            if (spawnPosition != Vector3.zero) // Kontrola, zda je pozice validní
            {
                int index = Random.Range(0, enemyPrefab.Length);
                GameObject enemy = Instantiate(enemyPrefab[index], spawnPosition, Quaternion.identity);
                activeEnemies.Add(enemy);
            }
        }
    }

    private void CheckMinionStatus()
    {
        activeEnemies.RemoveAll(minion => minion == null);

        if (activeEnemies.Count == 0)
        {
            if (currentWave <= maxNumberOfWaves)
            {
                hasAllMinionsDied = true;
                numberOfEnemies += enemyWaveAddition;
                currentWave++;

            }
            else
            {
                isFightActive = false;
                if (doorToNextLevel != null && !doorHasOpened)
                {
                    doorToNextLevel.GetComponent<BaseOpenDoor>().OpenDoor();
                    doorHasOpened = true;
                }

            }
        }
        else hasAllMinionsDied = false;
    }

    private Vector3 GetValidSpawnPosition(Bounds bounds)
    {
        int maxAttempts = 10; // Kolikrát se pokusíme najít validní pozici
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.min.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            // Zkontroluj, zda je pozice validní na NavMeshu
            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position; // Vrátí validní pozici na NavMeshu
            }
        }

        Debug.LogWarning("Nenalezl jsem validní pozici pro spawnování.");
        return Vector3.zero; // Vrátí nulu, pokud žádná pozice není validní
    }
}
