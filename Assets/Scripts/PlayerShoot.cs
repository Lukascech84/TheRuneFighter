using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bullet; // Prefab støely
    public float bulletLife = 1.5f; // Doba životnosti støely
    public float bulletSpeed = 6.5f; // Rychlost støely
    [SerializeField] private float firingRate = 0.04f; // Interval mezi støelami

    [SerializeField] private Vector3 spawnOffset = Vector3.zero; // Offset pro spawn støely

    private bool isShooting; // Indikátor, zda hráè støílí
    private float timer = 0f; // Èasovaè pro støelbu

    public void OnShoot(InputAction.CallbackContext context)
    {
        // Nastavení støelby podle vstupu
        if (context.performed) isShooting = true;
        else isShooting = false;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Pokud je èas na další støelu a hráè støílí
        if (timer >= firingRate && isShooting)
        {
            Fire(); // Vystøel støelu
            timer = 0f; // Resetuj èasovaè
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
            spawnedBullet.GetComponent<Bullet>().speed = bulletSpeed;
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