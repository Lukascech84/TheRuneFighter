using System.Collections;
using UnityEngine;

public class DeleteSubObjects : MonoBehaviour
{
    [SerializeField] private float minDelay = 2.0f; // Minimální zpoždìní
    [SerializeField] private float maxDelay = 5.0f; // Maximální zpoždìní

    void Start()
    {
        StartCoroutine(DeleteSubObjectsAndSelf());
    }

    IEnumerator DeleteSubObjectsAndSelf()
    {
        // Pro každý podobjekt spustí coroutine
        foreach (Transform child in transform)
        {
            StartCoroutine(DeleteAfterDelay(child.gameObject));
        }

        // Èeká, dokud jsou stále podobjekty
        while (transform.childCount > 0)
        {
            yield return null; // Poèkej jeden frame
        }

        // Smaže hlavní objekt
        Destroy(gameObject);
    }

    IEnumerator DeleteAfterDelay(GameObject obj)
    {
        // Náhodné zpoždìní v daném intervalu
        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        // Smaže podobjekt, pokud stále existuje
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}