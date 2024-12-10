using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoManager : MonoBehaviour
{
    public TextMeshProUGUI current;
    public TextMeshProUGUI max;
    public GameObject character;
    private PlayerShootWithMagazine atm;

    // Start is called before the first frame update
    void Start()
    {
        atm = character.GetComponent<PlayerShootWithMagazine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (atm == null)
        {
            current.text = "0";
            max.text = "0";
        }
        else
        {
            current.text = atm.currentAmmo.ToString();
            max.text = atm.magazineSize.ToString();
        }
    }
}
