using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashManager : MonoBehaviour
{

    public Image dashBar;
    private float dashAmount;
    public GameObject character;
    private AttributeManager atm;

    // Start is called before the first frame update
    void Start()
    {
        atm = character.GetComponent<AttributeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (atm == null)
        {
            dashBar.fillAmount = 0;
        }
        else
        {
            dashAmount = atm.dashCurrentCoolDown;
            dashBar.fillAmount = Mathf.Lerp(0, 1, Mathf.InverseLerp(atm.dashCooldown, 0, dashAmount)); ;
        }
    }
}
