using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletLife = 1f; // Doba �ivotnosti st�ely
    public float speed = 1f; // Rychlost st�ely

    private Vector3 direction; // Sm�r st�ely
    private Vector3 spawnPoint; // Po��te�n� bod st�ely
    private float timer = 0f;
    public GameObject spawner;
    public AttributeManager playerAtm;
    public AttributeManager enemyAtm;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;

    void Start()
    {
        // Ulo��me po��te�n� bod st�ely
        spawnPoint = transform.position;

        if(muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
        }
    }

    void Update()
    {
        // Zni��me st�elu po uplynut� �ivotnosti
        if (timer > bulletLife) Destroy(this.gameObject);

        // Aktualizujeme �asova�
        timer += Time.deltaTime;

        // Posuneme st�elu ve sm�ru jej�ho pohybu
        transform.position = Movement(timer);
    }

    public void SetDirection(Vector3 dir)
    {
        // Nastav�me sm�r st�ely (normalizovan�)
        direction = dir.normalized;
    }

    private Vector3 Movement(float timer)
    {
        // Vypo��t�me novou pozici st�ely na z�klad� sm�ru a rychlosti
        float x = timer * speed * direction.x;
        float z = timer * speed * direction.z;
        return new Vector3(x + spawnPoint.x, spawnPoint.y, z + spawnPoint.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawner != null)
        {
            playerAtm = spawner.GetComponent<AttributeManager>();
            enemyAtm = other.GetComponent<AttributeManager>();

            // Pokud kulka zas�hne sv�j p�vodce, ignorujeme kolizi
            if (other.gameObject == spawner)
            {
                return;
            }

            // Zkontrolujeme, zda zasa�en� objekt m� komponentu Health
            if (enemyAtm != null)
            {
                if (enemyAtm.health > 0f)
                {
                    playerAtm.DealDamage(enemyAtm.gameObject);
                }
            }
        }

        // Zni��me kulku po z�sahu
        if (other.CompareTag("Player") || other.CompareTag("room")) return;

        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, transform.position, Quaternion.identity);
            Destroy(hitVFX, 2f); // Optional: Zni��me efekt po 2 vte�in�ch
        }
        Destroy(gameObject);
    }
}