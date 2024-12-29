using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributeManager : BaseAttributeManager
{
    public float RangeDamage;
    public float MeleeDamage;
    public float MeleeAttackCooldown = 0.5f;
    public float dashCurrentCoolDown;
    public float dashDistance = 2f;
    public float dashCooldown = 2.5f;
    public float dashDuration = 0.2f;
}
