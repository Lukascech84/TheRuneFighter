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
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        if (screenPos.z > 0 && (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height))
        {
            indicator.SetActive(true);

            Vector3 direction = target.position - player.position;
            direction.y = 0; // Ignorujeme vertikální rozdíl

            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

            Vector3 clampedPosition = ClampToScreenEdge(screenPos);
            indicator.transform.position = clampedPosition;

            indicator.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        else
        {
            indicator.SetActive(false);
        }
    }

    Vector3 ClampToScreenEdge(Vector3 screenPos)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Vector2 direction = ((Vector2)screenPos - screenCenter).normalized;

        float maxX = Screen.width - 50;
        float maxY = Screen.height - 50;
        Vector3 edgePos = screenCenter + direction * Mathf.Min(maxX, maxY);

        return new Vector3(
            Mathf.Clamp(edgePos.x, 50, maxX),
            Mathf.Clamp(edgePos.y, 50, maxY),
            screenPos.z
        );
    }
}
