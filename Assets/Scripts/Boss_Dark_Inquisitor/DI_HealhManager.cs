using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DI_HealthManager : MonoBehaviour
{

    public Image healthBar;
    public Image easeHealthBar;
    private float healthAmount;
    public GameObject Boss;
    private DI_BossAttributeManager BossAtm;
    private float lerpSpeed = 0.025f;

    // Start is called before the first frame update
    void Start()
    {
        if (Boss == null)
        {
            enabled = false;
            return;
        }
        BossAtm =  Boss.GetComponent<DI_BossAttributeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        healthAmount = BossAtm.CurrentHealth / BossAtm.BaseHealth;
        if (BossAtm == null)
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
