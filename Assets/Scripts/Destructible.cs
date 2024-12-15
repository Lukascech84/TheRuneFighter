using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyedVersion;

    void Start()
    {
        GameObject tmp = Instantiate(destroyedVersion);
        tmp.SetActive(false);
        destroyedVersion = tmp;

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Bullet>() != null) {
            GameObject destroyed = destroyedVersion;
            if (destroyed != null)
            {
                destroyed.transform.position = transform.position;
                destroyed.transform.rotation = transform.rotation;
                destroyed.SetActive(true);
            }
            Destroy(gameObject);
        }
    }
}