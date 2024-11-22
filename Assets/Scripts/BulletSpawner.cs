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
    [SerializeField] private Vector3 spawnOffset = Vector3.zero; // Offset pro spawn st�ely

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
        if (bullet) // Zkontroluj, zda je p�i�azen prefab st�ely
        {
            // Oto�en� sm�ru o 90 stup�� kolem osy Y
            Quaternion adjustedRotation = transform.rotation * Quaternion.Euler(0, 0, 0);

            // Vypo��t�n� p�esn� pozice pro spawn st�ely s offsetem
            Vector3 spawnPosition = transform.position + transform.rotation * spawnOffset;

            // Vytvo�en� st�ely na posunut� pozici s upravenou rotac�
            GameObject spawnedBullet = Instantiate(bullet, spawnPosition, adjustedRotation);

            // Vypo��t�me nov� sm�r st�ely
            Vector3 bulletDirection = adjustedRotation * Vector3.forward;

            // P�ed�me sm�r a rychlost st�ely do komponenty Bullet
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