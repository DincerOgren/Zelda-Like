using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUISpawner : MonoBehaviour
{
    public WeaponUIController weaponUIController;
    [SerializeField] WeaponItemUI prefab;
    [SerializeField] WeaponItemUI defaultPrefab;
    TempInventory inventory;
    AttackController attackController;
    private int inventoryFillAmount;
    private void Awake()
    {
        inventory = TempInventory.GetPlayerInventory();
        attackController = AttackController.GetPlayerAttackController();
        inventory.inventoryUpdated += ReCompile;
    }
    void Start()
    {
        SpawnDefault();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ReCompile();
        }
    }

    private void SpawnDefault()
    {
        var a = Instantiate(defaultPrefab, transform);
        a.SetupDefault(inventory, attackController,weaponUIController);
    }
    private void ReCompile()
    {
        inventoryFillAmount = inventory.GetFilledAmount(SlotType.Weapon); ;
        print(inventoryFillAmount + " fill amount");
        var weaponInventory = inventory.GetInventoryFromType(SlotType.Weapon);
        
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        SpawnDefault();
        int j = 0;
        for (int i = 0; i < weaponInventory.inventorySize; i++)
        {
            if (j == inventoryFillAmount)
            {
                break;
            }

            if (weaponInventory.slots[i].item == null)
            {
                continue;
            }
            else
            {
                var itemUI = Instantiate(prefab, transform);
                itemUI.Setup(inventory, attackController,weaponUIController, j+1,i);
                j++;
            }
        }

    }
}
