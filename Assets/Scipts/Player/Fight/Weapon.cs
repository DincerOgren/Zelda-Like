using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform[] swordParts;
    

    public void DropEventForWeapon()
    {
        foreach (var item in swordParts)
        {
            item.GetComponent<Collider>().isTrigger= false;
        }
    }
}
