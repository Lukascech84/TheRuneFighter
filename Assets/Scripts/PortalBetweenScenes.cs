using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Portal : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneToLoadAsset; // Scéna v editoru
#endif
    [SerializeField] private string sceneToLoad; // Název scény bìhem runtime

    private void OnValidate()
    {
#if UNITY_EDITOR
        // Automaticky uloží název scény, když je scéna vybrána v editoru
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
            Debug.LogError("Není nastavena žádná scéna k naètení!");
        }
    }
}
