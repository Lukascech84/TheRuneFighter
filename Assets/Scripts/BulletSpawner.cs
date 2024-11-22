using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{

    enum SpawnerType { Straight, Spin }

    [Header("Bullet Attributes")]
    public GameObject bullet;
    public float bulletLife = 1.5f;
    public float speed = 6.5f;

    [Header("Spawner Attributes")]
    [SerializeField] private SpawnerType spawnerType;
    [SerializeField] private float firingRate = 0.04f;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero; // Offset pro spawn støely

    private float timer = 0f;

    void Start()
    {

    }

    void Update()
    {
        timer += Time.deltaTime;
        if (spawnerType == SpawnerType.Spin) transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y + 1f, 0f);
        if (timer >= firingRate)
        {
            Fire();
            timer = 0;
        }
    }

    private void Fire()
    {
        if (bullet) // Zkontroluj, zda je pøiøazen prefab støely
        {
            // Otoèení smìru o 90 stupòù kolem osy Y
            Quaternion adjustedRotation = transform.rotation * Quaternion.Euler(0, 0, 0);

            // Vypoèítání pøesné pozice pro spawn støely s offsetem
            Vector3 spawnPosition = transform.position + transform.rotation * spawnOffset;

            // Vytvoøení støely na posunuté pozici s upravenou rotací
            GameObject spawnedBullet = Instantiate(bullet, spawnPosition, adjustedRotation);

            // Vypoèítáme nový smìr støely
            Vector3 bulletDirection = adjustedRotation * Vector3.forward;

            // Pøedáme smìr a rychlost støely do komponenty Bullet
            spawnedBullet.GetComponent<Bullet>().speed = speed;
            spawnedBullet.GetComponent<Bullet>().bulletLife = bulletLife;
            spawnedBullet.GetComponent<Bullet>().SetDirection(bulletDirection);
            Bullet bulletScript = spawnedBullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.spawner = gameObject;
            }
        }
    }
}