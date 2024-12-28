using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIndicator : MonoBehaviour
{
    public Transform player; // Hráè
    public Camera mainCamera; // Hlavní kamera
    public RectTransform canvasRect; // Canvas UI
    public GameObject enemyIndicatorPrefab; // Prefab pro bìžného nepøítele
    public GameObject bossIndicatorPrefab; // Prefab pro bosse

    private List<GameObject> enemyIndicators = new List<GameObject>();
    private List<GameObject> bossIndicators = new List<GameObject>();

    void Update()
    {
        // Získání všech objektù na vrstvách Enemy a Boss
        GameObject[] enemies = GetObjectsInLayer(LayerMask.NameToLayer("enemy"));
        GameObject[] bosses = GetObjectsInLayer(LayerMask.NameToLayer("boss"));

        // Aktualizace ukazatelù pro nepøátele
        UpdateIndicators(enemies, enemyIndicators, enemyIndicatorPrefab);

        // Aktualizace ukazatelù pro bossy
        UpdateIndicators(bosses, bossIndicators, bossIndicatorPrefab);
    }

    GameObject[] GetObjectsInLayer(int layer)
    {
        // Najde všechny objekty na konkrétní vrstvì
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
                // Vytvoøení nového ukazatele
                GameObject newIndicator = Instantiate(prefab, canvasRect);
                indicators.Add(newIndicator);
            }

            UpdateIndicator(indicators[i], targets[i].transform);
        }

        // Deaktivace nepoužitých ukazatelù
        for (int i = targets.Length; i < indicators.Count; i++)
        {
            indicators[i].SetActive(false);
        }
    }

    void UpdateIndicator(GameObject indicator, Transform target)
    {
        // Pøevod svìtových souøadnic cíle do souøadnic obrazovky
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        // Pokud je z souøadnice záporná, nastavíme ji na 0.1, aby nepøítel nebyl za kamerou
        if (screenPos.z < 0)
        {
            screenPos *= -1; // Otoèíme smìr, pokud je nepøítel za kamerou
        }

        // Najdi smìr od støedu obrazovky k cíli
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 direction = ((Vector2)screenPos - screenCenter).normalized;

        // Zjistíme, zda je nepøítel mimo obrazovku
        bool isOffScreen = screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height;

        if (isOffScreen)
        {
            indicator.SetActive(true);

            // Pøichycení ukazatele na okraj obrazovky
            Vector3 clampedPosition = ClampToScreenEdge(screenPos);
            indicator.transform.position = clampedPosition;

            // Natoèení indikátoru smìrem k cíli
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            indicator.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Pokud je nepøítel viditelný na obrazovce, skryj indikátor
            indicator.SetActive(false);
        }
    }

    Vector3 ClampToScreenEdge(Vector3 screenPos)
    {
        // Støed obrazovky
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        // Smìr od støedu obrazovky
        Vector2 direction = ((Vector2)screenPos - screenCenter).normalized;

        // Maximální hodnoty okrajù obrazovky
        float halfWidth = Screen.width / 2f - 50; // 50 px okraj
        float halfHeight = Screen.height / 2f - 50;

        // Pomìr pro výpoèet clamped pozice
        float scaleFactor = Mathf.Min(
            Mathf.Abs(halfWidth / direction.x),
            Mathf.Abs(halfHeight / direction.y)
        );

        // Nová pozice na okraji obrazovky
        Vector3 edgePos = screenCenter + direction * scaleFactor;

        return new Vector3(
            Mathf.Clamp(edgePos.x, 50, Screen.width - 50),
            Mathf.Clamp(edgePos.y, 50, Screen.height - 50),
            screenPos.z
        );
    }
}
