using UnityEngine;
using System.Collections.Generic;

public class RaycastTransparency : MonoBehaviour
{
    public Transform origin;
    public Transform target;
    public float TransparencyLevel = 0f;
    public int TemporaryLayer = 8; // Nastavte zde doèasný layer (napø. 8)
    public LayerMask ignoreLayers; // Vrstva (nebo vrstvy), které mají být ignorovány

    private HashSet<GameObject> affectedObjects = new HashSet<GameObject>();
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - origin.position).normalized;
            float distance = Vector3.Distance(origin.position, target.position);

            RaycastHit[] hits = Physics.RaycastAll(origin.position, direction, distance, ~ignoreLayers);

            HashSet<GameObject> currentHits = new HashSet<GameObject>();

            foreach (RaycastHit hit in hits)
            {
                GameObject obj = hit.collider.gameObject;

                if (obj == null) continue; // Pøeskoèíme, pokud je objekt null

                currentHits.Add(obj);

                if (!affectedObjects.Contains(obj))
                {
                    if (!originalLayers.ContainsKey(obj))
                    {
                        originalLayers[obj] = obj.layer; // Uložit pùvodní layer
                    }

                    obj.layer = TemporaryLayer; // Nastavit doèasný layer

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

            // Obnovujeme layer a viditelnost pro objekty, které již nejsou zasaženy
            foreach (GameObject obj in affectedObjects)
            {
                if (obj == null) continue; // Pøeskoèíme, pokud je objekt null

                if (!currentHits.Contains(obj))
                {
                    if (originalLayers.ContainsKey(obj))
                    {
                        obj.layer = originalLayers[obj]; // Obnovení pùvodního layeru
                        originalLayers.Remove(obj);
                    }

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

            // Odstranìní znièených objektù z `originalLayers`
            foreach (var entry in new List<GameObject>(originalLayers.Keys))
            {
                if (entry == null)
                {
                    originalLayers.Remove(entry);
                }
            }

            affectedObjects = currentHits;
        }
    }

    void ResetMaterialVisibility(Material material)
    {
        if (material == null) return;

        material.SetFloat("_Surface", 0); // Opaque
        material.SetFloat("_ZWrite", 1);
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

        material.EnableKeyword("_SURFACE_TYPE_OPAQUE");
        material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");

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
            return;
        }

        material.SetFloat("_Surface", 1); // Transparent
        material.SetFloat("_Blend", 0);
        material.SetFloat("_ZWrite", 0);
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.DisableKeyword("_SURFACE_TYPE_OPAQUE");
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_BLENDMODE_ALPHA");

        Color color = material.color;
        color.a = Mathf.Clamp01(transparencyLevel);
        material.color = color;

        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
    }
}