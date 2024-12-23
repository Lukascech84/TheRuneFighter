using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DI_MagicProjectile : MonoBehaviour
{
    public float bulletLife;
    public float speed;

    private Vector3 direction;
    private Vector3 spawnPoint;
    private float timer = 0f;

    public PlayerAttributeManager playerAtm;
    public DI_BossAttributeManager bossAtm;

    public GameObject Spawner;
    public GameObject Player;

    public GameObject muzzlePrefab;
    public GameObject hitPrefab;

    void Start()
    {
        bossAtm = Spawner.GetComponent<DI_BossAttributeManager>();
        playerAtm = Player.GetComponent<PlayerAttributeManager>();

        bulletLife = bossAtm.ProjectileDamage;
        speed = bossAtm.ProjectileSpeed;

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
        if (Spawner != null)
        {
            if (other.gameObject == Spawner) return;
            if (other.gameObject.GetComponent<Bullet>() != null) return;

            if (other.gameObject == Player)
            {
                if (playerAtm != null)
                {
                    if (playerAtm.CurrentHealth > 0f)
                    {
                        bossAtm.DealDamage(playerAtm.gameObject, bossAtm.ProjectileDamage);
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
