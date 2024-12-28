using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIndicator : MonoBehaviour
{
    public Transform player; // Hr��
    public Camera mainCamera; // Hlavn� kamera
    public RectTransform canvasRect; // Canvas UI
    public GameObject enemyIndicatorPrefab; // Prefab pro b�n�ho nep��tele
    public GameObject bossIndicatorPrefab; // Prefab pro bosse

    private List<GameObject> enemyIndicators = new List<GameObject>();
    private List<GameObject> bossIndicators = new List<GameObject>();

    void Update()
    {
        // Z�sk�n� v�ech objekt� na vrstv�ch Enemy a Boss
        GameObject[] enemies = GetObjectsInLayer(LayerMask.NameToLayer("enemy"));
        GameObject[] bosses = GetObjectsInLayer(LayerMask.NameToLayer("boss"));

        // Aktualizace ukazatel� pro nep��tele
        UpdateIndicators(enemies, enemyIndicators, enemyIndicatorPrefab);

        // Aktualizace ukazatel� pro bossy
        UpdateIndicators(bosses, bossIndicators, bossIndicatorPrefab);
    }

    GameObject[] GetObjectsInLayer(int layer)
    {
        // Najde v�echny objekty na konkr�tn� vrstv�
        GameObject[] objects = FindObjectsOfType<GameObject>();
        List<GameObject> objectsInLayer = new List<GameObject>();

        foreach (GameObject obj in objects)
        {
            if (obj.layer == layer)
            {
                objectsInLayer.Add(obj);
            }
        }

        return objectsInLayer.ToArray();
    }

    void UpdateIndicators(GameObject[] targets, List<GameObject> indicators, GameObject prefab)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (i >= indicators.Count)
            {
                // Vytvo�en� nov�ho ukazatele
                GameObject newIndicator = Instantiate(prefab, canvasRect);
                indicators.Add(newIndicator);
            }

            UpdateIndicator(indicators[i], targets[i].transform);
        }

        // Deaktivace nepou�it�ch ukazatel�
        for (int i = targets.Length; i < indicators.Count; i++)
        {
            indicators[i].SetActive(false);
        }
    }

    void UpdateIndicator(GameObject indicator, Transform target)
    {
        // P�evod sv�tov�ch sou�adnic c�le do sou�adnic obrazovky
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        // Pokud je z sou�adnice z�porn�, nastav�me ji na 0.1, aby nep��tel nebyl za kamerou
        if (screenPos.z < 0)
        {
            screenPos *= -1; // Oto��me sm�r, pokud je nep��tel za kamerou
        }

        // Najdi sm�r od st�edu obrazovky k c�li
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = ((Vector2)screenPos - screenCenter).normalized;

        // Zjist�me, zda je nep��tel mimo obrazovku
        bool isOffScreen = screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height;

        if (isOffScreen)
        {
            indicator.SetActive(true);

            // P�ichycen� ukazatele na okraj obrazovky
            Vector3 clampedPosition = ClampToScreenEdge(screenPos);
            indicator.transform.position = clampedPosition;

            // Nato�en� indik�toru sm�rem k c�li
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            indicator.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Pokud je nep��tel viditeln� na obrazovce, skryj indik�tor
            indicator.SetActive(false);
        }
    }

    Vector3 ClampToScreenEdge(Vector3 screenPos)
    {
        // St�ed obrazovky
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        // Sm�r od st�edu obrazovky
        Vector2 direction = ((Vector2)screenPos - screenCenter).normalized;

        // Maxim�ln� hodnoty okraj� obrazovky
        float halfWidth = Screen.width / 2f - 50; // 50 px okraj
        float halfHeight = Screen.height / 2f - 50;

        // Pom�r pro v�po�et clamped pozice
        float scaleFactor = Mathf.Min(
            Mathf.Abs(halfWidth / direction.x),
            Mathf.Abs(halfHeight / direction.y)
        );

        // Nov� pozice na okraji obrazovky
        Vector3 edgePos = screenCenter + direction * scaleFactor;

        return new Vector3(
            Mathf.Clamp(edgePos.x, 50, Screen.width - 50),
            Mathf.Clamp(edgePos.y, 50, Screen.height - 50),
            screenPos.z
        );
    }
}
