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

    void Start()
    {
        // Ulo��me po��te�n� bod st�ely
        spawnPoint = transform.position;
    }

    void Update()
    {
        // Zni��me st�elu po uplynut� �ivotnosti
        if (timer > bulletLife) Destroy(this.gameObject);

        // Aktualizujeme �asova�
        timer += Time.deltaTime;

        // Posuneme st�elu ve sm�ru jej�ho pohybu
        transform.position = Movement(timer);
    }

    public void SetDirection(Vector3 dir)
    {
        // Nastav�me sm�r st�ely (normalizovan�)
        direction = dir.normalized;
    }

    private Vector3 Movement(float timer)
    {
        // Vypo��t�me novou pozici st�ely na z�klad� sm�ru a rychlosti
        float x = timer * speed * direction.x;
        float z = timer * speed * direction.z;
        return new Vector3(x + spawnPoint.x, spawnPoint.y, z + spawnPoint.z);
    }
}
