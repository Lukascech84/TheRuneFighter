using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DI_Traps : MonoBehaviour
{
    private float delay;
    private float explosionRadius;
    private float damage;

    private DI_BossAttributeManager bossAtm;
    private BaseAttributeManager hitAtm;

    public GameObject WarningPrefab;
    public GameObject Spawner;
    public GameObject Player;
    private Animator Animator;
    private GameObject TrapWarning;

    void Start()
    {
        if (Spawner == null || Player == null) return;

        bossAtm = Spawner.GetComponent<DI_BossAttributeManager>();

        delay = bossAtm.TrapDelayWhenSpawned;
        explosionRadius = bossAtm.TrapExplosionRadius;
        damage = bossAtm.TrapDamage;
        Animator = GetComponent<Animator>();

        TrapWarning = Instantiate(WarningPrefab, transform.position, Quaternion.Euler(90, 0, 0));

        StartCoroutine(ScaleWarningOverTime(TrapWarning, delay));

        Invoke("Explode", delay);
    }


    IEnumerator ScaleWarningOverTime(GameObject warning, float duration)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        while (elapsedTime < duration)
        {
            if (warning == null) // Check if the warning object still exists
            {
                yield break; // Exit the coroutine if the warning has been destroyed
            }

            // Interpolate between the initial and target scale
            float progress = elapsedTime / duration;
            warning.transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        if (warning != null) // Final check before setting the scale
        {
            warning.transform.localScale = targetScale;
        }
    }


    void Explode()
    {
        Animator.SetTrigger("Trap");

        if (TrapWarning != null)
        {
            Destroy(TrapWarning); // Bezpeènì zniè varování, pokud ještì existuje
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider enemy in hitColliders)
        {
            hitAtm = enemy.GetComponent<BaseAttributeManager>();
            if (hitAtm != null && hitAtm.CurrentHealth > 0f)
            {
                bossAtm.DealDamage(hitAtm.gameObject, damage);
            }
        }

        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}