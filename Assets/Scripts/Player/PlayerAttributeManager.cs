using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributeManager : BaseAttributeManager
{
    public float RangeDamage;
    public float MeleeDamage;
    public float MeleeAttackCooldown = 0.5f;
    [HideInInspector] public float dashCurrentCoolDown;
    public float dashDistance = 2f;
    public float dashCooldown = 2.5f;
    public float dashDuration = 0.2f;
    public float dashTrailRefreshRate = 0.1f;
    public float TrailDestroyDelay = 3f;
    private Animator Animator;

    public override void Start()
    {
        Animator = GetComponent<Animator>();
        base.Start();
    }

    protected override void Die()
    {
        Animator.SetTrigger("dead");
    }

    private void StopTime()
    {
        Time.timeScale = 0f;
        isDead = true;
    }
}
