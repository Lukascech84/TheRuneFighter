using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashManager : MonoBehaviour
{

    public Image dashBar;
    private float dashAmount;
    public GameObject character;
    private PlayerAttributeManager PlayerAtm;

    // Start is called before the first frame update
    void Start()
    {
        PlayerAtm = character.GetComponent<PlayerAttributeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerAtm == null)
        {
            dashBar.fillAmount = 0;
        }
        else
        {
            dashAmount = PlayerAtm.dashCurrentCoolDown;
            dashBar.fillAmount = Mathf.Lerp(0, 1, Mathf.InverseLerp(PlayerAtm.dashCooldown, 0, dashAmount)); ;
        }
    }
}
