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
        // Najde všechny renderery vèetnì tìch z podobjektù
        renderers.AddRange(GetComponentsInChildren<Renderer>());

        // Uloží originální materiály každého rendereru
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
            // Nastaví èervený materiál na všechny renderery
            foreach (var rend in renderers)
            {
                rend.material = flashRedMaterial;
            }

            yield return new WaitForSeconds(duration);

            // Obnoví pùvodní materiál pro všechny renderery
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