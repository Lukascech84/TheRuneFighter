using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeManager : MonoBehaviour
{
    public float health;
    public float attack;
    public float dashCurrentCoolDown;
    public float dashDistance = 2f;
    public float dashCooldown = 2.5f;
    public float dashDuration = 0.2f;


    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void DealDamage(GameObject target)
    {
        var atm = target.GetComponent<AttributeManager>();
        if (atm != null)
        {
            atm.TakeDamage(attack);
        }
    }

    

}