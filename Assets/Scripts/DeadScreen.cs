using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadScreen : MonoBehaviour
{
    public GameObject deadscreen;
    public GameObject character;
    private PlayerAttributeManager PlayerAtm;

    void Start()
    {
        character = GameObject.FindWithTag("player");
        PlayerAtm = character.GetComponent<PlayerAttributeManager>();
    }

    void Update()
    {
        if (PlayerAtm.deadScreen)
        {
            deadscreen.SetActive(true);
        }
    }

    public void Respawn()
    {
        deadscreen.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
}
