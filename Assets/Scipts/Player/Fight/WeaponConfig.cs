using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/New Weapon")]

public class WeaponConfig : Item
{

    public WeaponType weaponType;

    public float weaponDamage;
    public float weaponHealth;
    public float timeBetweenAttacks ,
        comboToAttack2Time=0.3f,
        comboToAttack3Time= 0.3f;

    public Weapon equippedPrefab;
    public bool hasProjectile = false;

    public GameObject projectilePrefab = null;
    public AnimatorOverrideController animatorOverrider;

    const string weaponName = "Weapon";

    public Weapon Spawn(Transform rightHand)
    {
        DestroyOldWeapon(rightHand);

        Weapon weapon = null;
        if (equippedPrefab != null)
        {
            

            weapon = Instantiate(equippedPrefab, rightHand);
            weapon.name = weaponName;
        }
        //var overrideController = anim.runtimeAnimatorController as AnimatorOverrideController;
        //if (animatorOverrider != null)
        //{
        //    anim.runtimeAnimatorController = animatorOverrider;

        //}
        //else if (overrideController)
        //{
        //    anim.runtimeAnimatorController = overrideController.runtimeAnimatorController;
        //}

        return weapon;
    }


    private void DestroyOldWeapon(Transform rightHand)
    {
        Transform oldWeapon = rightHand.Find(weaponName);
        if (oldWeapon == null) return;

        Debug.Log("Destroying: " + oldWeapon.name);
        oldWeapon.name = "Destroy";
        Destroy(oldWeapon.gameObject);
    }



    public bool HasProjectile()
    {
        return projectilePrefab != null;
    }

    public override void Use()
    {
        base.Use();
        Debug.Log("WEAPON ITEM NAME:     :");
    }
    public override void Drop()
    {
        base.Drop();
        Debug.Log("WEAPON ITM DROP: : : :");
    }
}

