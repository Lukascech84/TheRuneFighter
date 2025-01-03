using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Portal : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneToLoadAsset; // Sc�na v editoru
#endif
    [SerializeField] private string sceneToLoad; // N�zev sc�ny b�hem runtime

    private void OnValidate()
    {
#if UNITY_EDITOR
        // Automaticky ulo�� n�zev sc�ny, kdy� je sc�na vybr�na v editoru
        if (sceneToLoadAsset != null)
        {
            sceneToLoad = sceneToLoadAsset.name;
        }
#endif
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Nen� nastavena ��dn� sc�na k na�ten�!");
        }
    }
}
