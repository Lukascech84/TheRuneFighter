using System.Collections;
using UnityEngine;

public class DeleteSubObjects : MonoBehaviour
{
    [SerializeField] private float minDelay = 2.0f; // Minim�ln� zpo�d�n�
    [SerializeField] private float maxDelay = 5.0f; // Maxim�ln� zpo�d�n�

    void Start()
    {
        StartCoroutine(DeleteSubObjectsAndSelf());
    }

    IEnumerator DeleteSubObjectsAndSelf()
    {
        // Pro ka�d� podobjekt spust� coroutine
        foreach (Transform child in transform)
        {
            StartCoroutine(DeleteAfterDelay(child.gameObject));
        }

        // �ek�, dokud jsou st�le podobjekty
        while (transform.childCount > 0)
        {
            yield return null; // Po�kej jeden frame
        }

        // Sma�e hlavn� objekt
        Destroy(gameObject);
    }

    IEnumerator DeleteAfterDelay(GameObject obj)
    {
        // N�hodn� zpo�d�n� v dan�m intervalu
        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        // Sma�e podobjekt, pokud st�le existuje
        if (obj != null)
        {
            Destroy(obj);
        }
    }
}