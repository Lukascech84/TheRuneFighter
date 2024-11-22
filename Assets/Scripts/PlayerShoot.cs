using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bullet; // Prefab st�ely
    public float bulletLife = 1.5f; // Doba �ivotnosti st�ely
    public float bulletSpeed = 6.5f; // Rychlost st�ely
    [SerializeField] private float firingRate = 0.04f; // Interval mezi st�elami

    [SerializeField] private Vector3 spawnOffset = Vector3.zero; // Offset pro spawn st�ely

    private bool isShooting; // Indik�tor, zda hr�� st��l�
    private float timer = 0f; // �asova� pro st�elbu

    public void OnShoot(InputAction.CallbackContext context)
    {
        // Nastaven� st�elby podle vstupu
        if (context.performed) isShooting = true;
        else isShooting = false;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Pokud je �as na dal�� st�elu a hr�� st��l�
        if (timer >= firingRate && isShooting)
        {
            Fire(); // Vyst�el st�elu
            timer = 0f; // Resetuj �asova�
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