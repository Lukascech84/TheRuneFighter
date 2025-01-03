using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAttributeManager : BaseAttributeManager
{
    public float damage;
    public float delay = 1f;
    public LayerMask enemyLayer = 8;


    public override void Start()
    {
        isInvincible = true;
        base.Start();
        Damage = damage;
    }

    public override void TakeDamage(float dmg)
    {
        if (isInvincible) return;
    }
}
