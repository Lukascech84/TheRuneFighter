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
    public DI_BossAttributeManager DIAtm;

    public GameObject muzzlePrefab;
    public GameObject hitPrefab;

    void Start()
    {
        spawnPoint = transform.position;

        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
        }
    }

    void Update()
    {
        if (timer > bulletLife) Destroy(this.gameObject);

        timer += Time.deltaTime;

        transform.position = Movement(timer);
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    private Vector3 Movement(float timer)
    {
        float x = timer * speed * direction.x;
        float z = timer * speed * direction.z;
        return new Vector3(x + spawnPoint.x, spawnPoint.y, z + spawnPoint.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spawner != null)
        {
            playerAtm = spawner.GetComponent<AttributeManager>();

            if (other.GetComponent<AttributeManager>() != null)
            {
                
                enemyAtm = other.GetComponent<AttributeManager>();

                if (other.gameObject == spawner) return;

                if (enemyAtm != null)
                {
                    if (enemyAtm.health > 0f)
                    {
                        playerAtm.DealDamage(enemyAtm.gameObject, playerAtm.RangeDamage);
                    }
                }
            }
            else if(other.GetComponent<DI_BossAttributeManager>() != null)
            {
                DIAtm = other.GetComponent<DI_BossAttributeManager>();

                if (other.gameObject == spawner) return;

                if (DIAtm != null)
                {
                    if (DIAtm.CurrentHealth > 0f)
                    {
                        DIAtm.TakeDamage(playerAtm.RangeDamage);
                    }
                }
            }
        }

        if (other.CompareTag("room")) return;

        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, transform.position, Quaternion.identity);
            Destroy(hitVFX, 2f);
        }
        Destroy(gameObject);
    }
}
