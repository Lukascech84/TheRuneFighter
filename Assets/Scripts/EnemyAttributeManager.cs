using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttributeManager : BaseAttributeManager
{
    public float damage;


    public override void Start()
    {
        base.Start();
        Damage = damage;
    }
}
