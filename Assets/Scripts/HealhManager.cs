using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{

    public Image healthBar;
    public Image easeHealthBar;
    private float healthAmount;
    public GameObject character;
    private AttributeManager player;
    private float lerpSpeed = 0.025f;

    // Start is called before the first frame update
    void Start()
    {
       player =  character.GetComponent<AttributeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        healthAmount = player.health / 100f;
        if (player == null)
        {
            healthBar.fillAmount = 0;
        }
        else if(healthBar.fillAmount != healthAmount)
        {
            healthBar.fillAmount = healthAmount;
        }

        if(healthBar.fillAmount != easeHealthBar.fillAmount)
        {
            easeHealthBar.fillAmount = Mathf.Lerp(easeHealthBar.fillAmount, healthAmount, lerpSpeed);
        }
        
        
    }
}
