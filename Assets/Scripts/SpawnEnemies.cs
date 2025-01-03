using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject[] enemyPrefab;
    private Collider ArenaBounds;
    public int numberOfEnemies = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool hasAllMinionsDied = false;
    public int enemyWaveAddition = 2;

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject == CompareTag("player"))
        {
            if(hasAllMinionsDied) SummonMinions();
        }
        CheckMinionStatus();
    }

    private void SummonMinions()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInArena(ArenaBounds.bounds);
            int index = Random.Range(0, enemyPrefab.Length);
            GameObject enemy = Instantiate(enemyPrefab[index], spawnPosition, Quaternion.identity);
            activeEnemies.Add(enemy);
        }
    }

    private void CheckMinionStatus()
    {
        activeEnemies.RemoveAll(minion => minion == null);

        if (activeEnemies.Count == 0)
        {
            hasAllMinionsDied = true;
            numberOfEnemies += enemyWaveAddition;
        }
    }

    private Vector3 GetRandomPositionInArena(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x), 0, Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
