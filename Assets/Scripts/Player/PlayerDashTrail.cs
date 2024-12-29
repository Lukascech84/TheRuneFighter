using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashTrail : MonoBehaviour
{
    private PlayerController PlayerController;
    private PlayerAttributeManager PlayerAtm;

    private bool isTrailActive;
    private float meshRefreshRate;
    private float meshDestroyDelay;
    public Material mat;
    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    // Seznam èástí, které budou generovat trail
    public List<SkinnedMeshRenderer> trailParts = new List<SkinnedMeshRenderer>();

    private void Start()
    {
        PlayerController = GetComponent<PlayerController>();
        PlayerAtm = GetComponent<PlayerAttributeManager>();
        meshRefreshRate = PlayerAtm.dashTrailRefreshRate;
        meshDestroyDelay = PlayerAtm.TrailDestroyDelay;

        // Ovìøte, zda byl seznam naplnìn
        if (trailParts.Count == 0)
        {
            Debug.LogWarning("No trail parts assigned! Please add SkinnedMeshRenderers to the trailParts list.");
        }
    }

    private void Update()
    {
        if (PlayerController.isDashing && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(PlayerAtm.dashDuration));
        }
    }

    IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            foreach (var skinnedMeshRenderer in trailParts)
            {
                if (skinnedMeshRenderer == null || !skinnedMeshRenderer.enabled)
                {
                    Debug.LogWarning($"Skipped renderer: {skinnedMeshRenderer?.name ?? "null"}");
                    continue;
                }

                Mesh mesh = new Mesh();
                skinnedMeshRenderer.BakeMesh(mesh);

                if (mesh.vertexCount == 0)
                {
                    Debug.LogWarning($"Mesh from {skinnedMeshRenderer.name} has no vertices. Skipping.");
                    continue;
                }

                GameObject trail = new GameObject("Trail");
                trail.transform.SetPositionAndRotation(skinnedMeshRenderer.transform.position, skinnedMeshRenderer.transform.rotation);
                trail.transform.localScale = skinnedMeshRenderer.transform.lossyScale * 0.1f;

                MeshRenderer mr = trail.AddComponent<MeshRenderer>();
                MeshFilter mf = trail.AddComponent<MeshFilter>();

                mf.mesh = mesh;
                mr.material = mat;

                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(trail, meshDestroyDelay);
                Destroy(mesh, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }

        isTrailActive = false;
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat(shaderVarRef);

        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
