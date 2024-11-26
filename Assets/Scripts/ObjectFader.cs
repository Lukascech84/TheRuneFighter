using UnityEngine;
using System.Collections.Generic;

public class RaycastTransparency : MonoBehaviour
{
    public Transform origin;
    public Transform target;
    public float TransparencyLevel = 0f;

    private HashSet<GameObject> affectedObjects = new HashSet<GameObject>();

    void Update()
    {
        // Vzdálenost a smìr paprsku
        if (target != null)
        {
            Vector3 direction = (target.position - origin.position).normalized;
            float distance = Vector3.Distance(origin.position, target.position);
        
        // Provádíme Raycast
        RaycastHit[] hits = Physics.RaycastAll(origin.position, direction, distance);

        // Nová množina aktuálnì zasažených objektù
        HashSet<GameObject> currentHits = new HashSet<GameObject>();

        foreach (RaycastHit hit in hits)
        {
            GameObject obj = hit.collider.gameObject;
            currentHits.Add(obj);

            // Pokud objekt ještì nebyl ovlivnìn, nastavíme prùhlednost
            if (!affectedObjects.Contains(obj))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    foreach (Material mat in renderer.materials)
                    {
                        SetMaterialTransparent(mat, TransparencyLevel);
                    }
                }
            }
        }

        // Obnovujeme viditelnost pro objekty, které nejsou aktuálnì zasaženy
        foreach (GameObject obj in affectedObjects)
        {
            if (obj != null) {
            if (!currentHits.Contains(obj))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    foreach (Material mat in renderer.materials)
                    {
                        ResetMaterialVisibility(mat);
                    }
                }
                }
            }
        }

        // Aktualizujeme seznam ovlivnìných objektù
        affectedObjects = currentHits;

        // Debug vizualizace
        //Debug.DrawLine(origin.position, target.position, Color.red);
        }
    }

    void ResetMaterialVisibility(Material material)
    {
        if (material == null) return;

        // Pøepnutí na Opaque mód
        material.SetFloat("_Surface", 0); // Opaque
        material.SetFloat("_ZWrite", 1); // Povolit zápis do Z-bufferu
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

        // Aktivace klíèových slov pro neprùhledný mód
        material.EnableKeyword("_SURFACE_TYPE_OPAQUE");
        material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");

        // Obnovíme alpha kanál na plnou viditelnost
        Color color = material.color;
        color.a = 1f;
        material.color = color;
    }

    void SetMaterialTransparent(Material material, float transparencyLevel)
    {
        if (material == null) return;

        if (material.shader.name != "Universal Render Pipeline/Lit")
        {
            material.shader = Shader.Find("Universal Render Pipeline/Lit");
        }

        if (material.shader == null)
        {
            //Debug.LogError("Shader 'Universal Render Pipeline/Lit' not found. Make sure URP is correctly set up.");
            return;
        }

        // Pøepnutí na transparentní režim
        material.SetFloat("_Surface", 1); // Transparent (0 = Opaque, 1 = Transparent)
        material.SetFloat("_Blend", 0);   // Alpha blending
        material.SetFloat("_ZWrite", 0);  // Zakázat zápis do Z-bufferu
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // Aktivujeme prùhlednost a deaktivujeme nepoužívané vlastnosti
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.DisableKeyword("_SURFACE_TYPE_OPAQUE");
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_BLENDMODE_ALPHA");

        // Nastavení alpha kanálu na prùhlednost
        Color color = material.color;
        color.a = Mathf.Clamp01(transparencyLevel); // Plnì prùhledný
        material.color = color;

        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
    }
}

/* using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    public Transform origin;
    public Transform target;

    void Update()
    {
        float distance = Vector3.Distance(origin.position, target.position);

        Vector3 direction = (target.position - origin.position).normalized;

        RaycastHit[] hits = Physics.RaycastAll(origin.position, direction, distance);

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();

            if (renderer != null)
            {
                foreach (Material mat in renderer.materials)
                {
                    SetMaterialTransparent(mat);
                }
            }
        }

        //Debug.DrawLine(origin.position, target.position, Color.red);
    }

    void SetMaterialTransparent(Material material)
    {
        if (material == null) return;

        if (material.shader.name != "Universal Render Pipeline/Lit")
        {
            material.shader = Shader.Find("Universal Render Pipeline/Lit");
        }

        if (material.shader == null)
        {
            //Debug.LogError("Shader 'Universal Render Pipeline/Lit' not found. Make sure URP is correctly set up.");
            return;
        }

        // Pøepnutí na transparentní režim
        material.SetFloat("_Surface", 1); // Transparent (0 = Opaque, 1 = Transparent)
        material.SetFloat("_Blend", 0);   // Alpha blending
        material.SetFloat("_ZWrite", 0);  // Zakázat zápis do Z-bufferu
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // Aktivujeme prùhlednost a deaktivujeme nepoužívané vlastnosti
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.DisableKeyword("_SURFACE_TYPE_OPAQUE");
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_BLENDMODE_ALPHA");

        // Nastavení alpha kanálu na prùhlednost
        Color color = material.color;
        color.a = 0f; // Plnì prùhledný
        material.color = color;
    }
}
*/