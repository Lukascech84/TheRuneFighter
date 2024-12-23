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
    private PlayerAttributeManager PlayerAtm;
    private float lerpSpeed = 0.025f;

    // Start is called before the first frame update
    void Start()
    {
        PlayerAtm =  character.GetComponent<PlayerAttributeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        healthAmount = PlayerAtm.CurrentHealth / 100f;
        if (PlayerAtm == null)
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
