using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMainCombatScript : MonoBehaviour
{
    private PlayerAttributeManager PlayerAtm;

    public PlayerCombatRanged RangedScript;
    public GameObject RangedWeapon;

    public PlayerCombatMelee MeleeScript;
    public GameObject MeleeWeaponHolster;
    public GameObject MeleeWeaponOut;

    public void SwitchToRanged(InputAction.CallbackContext context)
    {
        if (context.performed) SwitchToRanged();
    }

    public void SwitchToMelee(InputAction.CallbackContext context)
    {
        if (context.performed) SwitchToMelee();
    }

    void Start()
    {
        PlayerAtm = GetComponent<PlayerAttributeManager>();

        SwitchToRanged();
    }

    void SwitchToRanged()
    {
        //RangedWeapon.SetActive(true);
        PlayerAtm.Damage = PlayerAtm.RangeDamage;
        RangedScript.enabled = true;
        MeleeWeaponHolster.SetActive(true);
        MeleeWeaponOut.SetActive(false);
        MeleeScript.enabled = false;
    }

    void SwitchToMelee()
    {
        //RangedWeapon.SetActive(false);
        PlayerAtm.Damage = PlayerAtm.MeleeDamage;
        RangedScript.enabled = false;
        MeleeWeaponHolster.SetActive(false);
        MeleeWeaponOut.SetActive(true);
        MeleeScript.enabled = true;
    }
}
