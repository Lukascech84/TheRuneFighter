using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashRed : MonoBehaviour
{

    public Material flashRedMaterial;
    public float flashDuration = 0.05f;
    private new Renderer renderer;
    private Material originalMat;

    private GameObject spawner;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        originalMat = renderer.material;
    }

    private void OnTriggerEnter(Collider other)
    {
        spawner = other.GetComponent<Bullet>().spawner;

        if (other.GetComponent<Bullet>() != null) {
            if (spawner == gameObject) return;
            StartCoroutine(Flash(flashDuration));
            }
        }


    private IEnumerator Flash(float duration)
    {
        if (renderer != null)
        {
            renderer.material = flashRedMaterial;
            yield return new WaitForSeconds(duration);
            renderer.material = originalMat;
        }
    }
}
