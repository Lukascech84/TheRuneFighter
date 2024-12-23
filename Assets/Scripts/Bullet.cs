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
    public BaseAttributeManager spawnerAtm;
    public BaseAttributeManager enemyAtm;

    public GameObject muzzlePrefab;
    public GameObject hitPrefab;

    void Start()
    {
        spawnPoint = transform.position;

        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            Destroy(muzzleVFX, 2);
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
            spawnerAtm = spawner.GetComponent<BaseAttributeManager>();

            if (other.GetComponent<BaseAttributeManager>() != null)
            {   
                enemyAtm = other.GetComponent<BaseAttributeManager>();

                if (other.gameObject == spawner) return;

                if (enemyAtm != null)
                {
                    if (enemyAtm.CurrentHealth > 0f)
                    {
                        spawnerAtm.DealDamage(enemyAtm.gameObject, spawnerAtm.Damage);
                    }
                }
            }
        }

        if (other.CompareTag("room")) return;
        if (other.gameObject.GetComponent<DI_MagicProjectile>() != null) return;

        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, transform.position, Quaternion.identity);
            Destroy(hitVFX, 2f);
        }
        Destroy(gameObject);
    }
}
