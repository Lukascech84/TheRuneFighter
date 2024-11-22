using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletLife = 1f; // Doba životnosti støely
    public float speed = 1f; // Rychlost støely

    private Vector3 direction; // Smìr støely
    private Vector3 spawnPoint; // Poèáteèní bod støely
    private float timer = 0f;
    public GameObject spawner;
    public AttributeManager playerAtm;
    public AttributeManager enemyAtm;

    void Start()
    {
        // Uložíme poèáteèní bod støely
        spawnPoint = transform.position;
    }

    void Update()
    {
        // Znièíme støelu po uplynutí životnosti
        if (timer > bulletLife) Destroy(this.gameObject);

        // Aktualizujeme èasovaè
        timer += Time.deltaTime;

        // Posuneme støelu ve smìru jejího pohybu
        transform.position = Movement(timer);
    }

    public void SetDirection(Vector3 dir)
    {
        // Nastavíme smìr støely (normalizovaný)
        direction = dir.normalized;
    }

    private Vector3 Movement(float timer)
    {
        // Vypoèítáme novou pozici støely na základì smìru a rychlosti
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
            // Pokud kulka zasáhne svùj pùvodce, ignorujeme kolizi
            if (other.gameObject == spawner)
            {
                return;
            }

            // Zkontrolujeme, zda zasažený objekt má komponentu Health
            if (enemyAtm != null)
            {
                if (enemyAtm.health > 0f)
                {
                    playerAtm.DealDamage(enemyAtm.gameObject);
                    Debug.Log(playerAtm.gameObject + " Udìlil damage " + playerAtm.attack + " objektu " + enemyAtm.gameObject);
                }
                // Znièíme kulku po zásahu
                Destroy(gameObject);
            }
        }
    }
}
