using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    //TODO:equip weapon from inventory

    Weapon currentWeapon;

    public WeaponConfig defWeapon;
    public Transform rightHandTransform;

    public WeaponConfig currentConfig = null;

    public InventoryUsePanelControl usePanel;
    private TempInventory inventory;
    private void Start()
    {
        
        inventory=GetComponent<TempInventory>();
        currentWeapon = AttachWeapon(defWeapon);
        usePanel.dropEvent += UpdateWeapon;
    }


    private void Update()
    {
        //currentWeapon=inventory.GetEquippedWeapon();

       
    }
    public void EquipWeapon(WeaponConfig weapon)
    {
        currentConfig = weapon;
        currentWeapon = AttachWeapon(weapon);
    }
    private Weapon AttachWeapon(WeaponConfig weapon)
    {
        //Animator anim = GetComponent<Animator>();
        return weapon.Spawn(rightHandTransform);
    }
    public Weapon GetEquippedWeapon()
    {
        return currentWeapon;
    }

    private void UpdateWeapon()
    {
        var weapon = usePanel.GetActiveSlot().GetItem() as WeaponConfig;
        if (weapon == null)
        {
            EquipWeapon(defWeapon);
        }
        else
        {
            EquipWeapon(weapon);
        }

    }

}

public enum AttackType
{
    normal,
    heavy,
    charge
}