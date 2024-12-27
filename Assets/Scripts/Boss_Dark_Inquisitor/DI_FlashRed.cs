using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DI_FlashRed : MonoBehaviour
{
    public Material flashRedMaterial;
    public float flashDuration = 0.05f;

    private List<Renderer> renderers = new List<Renderer>();
    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private GameObject spawner;

    private void Start()
    {
        // Najde v�echny renderery v�etn� t�ch z podobjekt�
        renderers.AddRange(GetComponentsInChildren<Renderer>());

        // Ulo�� origin�ln� materi�ly ka�d�ho rendereru
        foreach (var rend in renderers)
        {
            originalMaterials[rend] = rend.material;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            spawner = bullet.spawner;
            if (spawner == gameObject) return;
            StartCoroutine(Flash(flashDuration));
        }
    }

    private IEnumerator Flash(float duration)
    {
        if (renderers.Count > 0)
        {
            // Nastav� �erven� materi�l na v�echny renderery
            foreach (var rend in renderers)
            {
                rend.material = flashRedMaterial;
            }

            yield return new WaitForSeconds(duration);

            // Obnov� p�vodn� materi�l pro v�echny renderery
            foreach (var rend in renderers)
            {
                if (originalMaterials.ContainsKey(rend))
                {
                    rend.material = originalMaterials[rend];
                }
            }
        }
    }
}