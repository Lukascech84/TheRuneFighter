using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttributeManager : MonoBehaviour
{
    public float BaseHealth;
    [HideInInspector] public float Damage;
    [HideInInspector] public float CurrentHealth;
    [HideInInspector] public bool isInvincible = false;

    public virtual void Start()
    {
        CurrentHealth = BaseHealth;
    }

    public virtual void TakeDamage(float dmg)
    {
        if (isInvincible) return;
        CurrentHealth -= dmg;
        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    public void DealDamage(GameObject target, float damage)
    {
        var atm = target.GetComponent<BaseAttributeManager>();
        if (atm != null)
        {
            atm.TakeDamage(damage);
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
