using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCombatUI : MonoBehaviour
{
    public WeaponConfig weaponConfig;
    public bool isChosen = false;
    private bool isEquipped = false;
    private AttackController attackController;
    private void Awake()
    {
        attackController = GameObject.FindWithTag("Player").GetComponent<AttackController>();
    }

    private void Update()
    {
        //TODO ONLY ONCE;
        if (isChosen && weaponConfig!=null && !isEquipped)
        {
            isEquipped = true;
            attackController.EquipWeapon(weaponConfig);
        }

    }
    public void Set()
    {
        isChosen = true; 
    }
    public void Drop()
    {
        isChosen = false;
        isEquipped = false;
    }
}
