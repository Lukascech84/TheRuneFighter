using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{

    public Image healthBar;
    private float healthAmount;
    public GameObject character;
    private AttributeManager player;

    // Start is called before the first frame update
    void Start()
    {
       player =  character.GetComponent<AttributeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            healthBar.fillAmount = 0;
        }
        else
        {
            healthAmount = player.health;
            healthBar.fillAmount = healthAmount / 100f;
        }
        
        
    }
}
