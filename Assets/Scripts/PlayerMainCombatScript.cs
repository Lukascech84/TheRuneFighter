using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMainCombatScript : MonoBehaviour
{
    private PlayerAttributeManager PlayerAtm;

    public PlayerCombatRanged RangedScript;
    public GameObject RangedWeapon;

    public PlayerCombatMelee MeleeScript;
    public GameObject MeleeWeapon;

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

        MeleeScript = GetComponent<PlayerCombatMelee>();
        RangedScript = GetComponent<PlayerCombatRanged>();
        SwitchToRanged();
    }

    void SwitchToRanged()
    {
        //RangedWeapon.SetActive(true);
        PlayerAtm.Damage = PlayerAtm.RangeDamage;
        RangedScript.enabled = true;
        MeleeWeapon.SetActive(false);
        MeleeScript.enabled = false;
    }

    void SwitchToMelee()
    {
        //RangedWeapon.SetActive(false);
        PlayerAtm.Damage = PlayerAtm.MeleeDamage;
        RangedScript.enabled = false;
        MeleeWeapon.SetActive(true);
        MeleeScript.enabled = true;
    }
}
