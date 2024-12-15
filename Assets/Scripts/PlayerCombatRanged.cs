using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatRanged: MonoBehaviour
{
    public GameObject bullet; // Prefab st�ely
    public float bulletLife = 1.5f; // Doba �ivotnosti st�ely
    public float bulletSpeed = 6.5f; // Rychlost st�ely
    [SerializeField] private float firingRate = 0.04f; // Interval mezi st�elami
    [SerializeField] private Vector3 spawnOffset = Vector3.zero; // Offset pro spawn st�ely

    // Z�sobn�k
    [SerializeField] public int magazineSize = 30; // Kapacita z�sobn�ku
    [SerializeField] private float reloadTime = 2f; // Doba p�eb�jen�

    private bool isShooting; // Indik�tor, zda hr�� st��l�
    private bool isReloading = false; // Indik�tor, zda prob�h� p�eb�jen�
    public int currentAmmo; // Aktu�ln� po�et n�boj� v z�sobn�ku
    private float timer = 0f; // �asova� pro st�elbu


    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed) isShooting = true;
        else isShooting = false;
    }


    private void Start()
    {
        currentAmmo = magazineSize; // Na za��tku m� z�sobn�k pln� po�et n�boj�
    }


    private void Update()
    {
        if (isReloading || Time.timeScale == 0) return; // Pokud p�eb�j�me, zastav�me v�echny ostatn� akce

        timer += Time.deltaTime;

        if (timer >= firingRate && isShooting && currentAmmo > 0)
        {
            Fire();
            timer = 0f;
        }

        // Automatick� p�eb�jen� p�i nulov�ch n�boj�ch
        if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }

        // Manu�ln� p�eb�jen�
        if (Keyboard.current.rKey.wasPressedThisFrame && !isReloading && currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
        }
    }

    private void Fire()
    {
        if (bullet)
        {
            Quaternion adjustedRotation = transform.rotation * Quaternion.Euler(0, 0, 0);
            Vector3 spawnPosition = transform.position + transform.rotation * spawnOffset;

            GameObject spawnedBullet = Instantiate(bullet, spawnPosition, adjustedRotation);

            Vector3 bulletDirection = adjustedRotation * Vector3.forward;

            Bullet bulletScript = spawnedBullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.speed = bulletSpeed;
                bulletScript.bulletLife = bulletLife;
                bulletScript.SetDirection(bulletDirection);
                bulletScript.spawner = gameObject;
            }

            currentAmmo--; // Sn�en� n�boj� v z�sobn�ku
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        //Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        isReloading = false;
        //Debug.Log("Reload complete!");
    }
}
