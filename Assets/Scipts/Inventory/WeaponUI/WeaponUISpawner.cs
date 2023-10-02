using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUISpawner : MonoBehaviour
{
    [SerializeField] MeleeCombatUI prefab;
    TempInventory inventory;


    private int inventorySize;
    private void Awake()
    {
        inventory = TempInventory.GetPlayerInventory();
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void Spawn()
    {
        inventorySize= inventory.GetInventoryFromType(SlotType.Weapon).inventorySize;

        for (int i = 0; i < inventorySize; i++)
        {
            Instantiate(prefab, transform);
        }
    }

    private void ReCompile()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }


        for (int i = 0; i < inventorySize; i++)
        {
            var itemUI = transform.GetChild(i).GetComponent<MeleeCombatUI>();
            
            //var itemUI = Instantiate(InventoryItemPrefab, transform);
        }
    }
}
