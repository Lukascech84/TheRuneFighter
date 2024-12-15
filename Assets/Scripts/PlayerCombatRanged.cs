using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatRanged: MonoBehaviour
{
    public GameObject bullet; // Prefab støely
    public float bulletLife = 1.5f; // Doba životnosti støely
    public float bulletSpeed = 6.5f; // Rychlost støely
    [SerializeField] private float firingRate = 0.04f; // Interval mezi støelami
    [SerializeField] private Vector3 spawnOffset = Vector3.zero; // Offset pro spawn støely

    // Zásobník
    [SerializeField] public int magazineSize = 30; // Kapacita zásobníku
    [SerializeField] private float reloadTime = 2f; // Doba pøebíjení

    private bool isShooting; // Indikátor, zda hráè støílí
    private bool isReloading = false; // Indikátor, zda probíhá pøebíjení
    public int currentAmmo; // Aktuální poèet nábojù v zásobníku
    private float timer = 0f; // Èasovaè pro støelbu


    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed) isShooting = true;
        else isShooting = false;
    }


    private void Start()
    {
        currentAmmo = magazineSize; // Na zaèátku má zásobník plný poèet nábojù
    }


    private void Update()
    {
        if (isReloading || Time.timeScale == 0) return; // Pokud pøebíjíme, zastavíme všechny ostatní akce

        timer += Time.deltaTime;

        if (timer >= firingRate && isShooting && currentAmmo > 0)
        {
            Fire();
            timer = 0f;
        }

        // Automatické pøebíjení pøi nulových nábojích
        if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }

        // Manuální pøebíjení
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

            currentAmmo--; // Snížení nábojù v zásobníku
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
