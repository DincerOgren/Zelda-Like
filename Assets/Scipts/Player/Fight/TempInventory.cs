using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class TempInventory : MonoBehaviour
{
    public Canvas weaponUI;
    public Canvas inventoryCanvas;
    public WeaponUIController uiCheck;
    private bool isInventoryOpen = false;

    [Header("Inventory Size")]

    public int inventorySize = 8;
    public InventoryUISlot[] slots;
    private AttackController attackController;


    [Serializable]
    public struct InventoryType
    {
        public SlotType inventoryType;
        public InventoryUISlot[] slots;
        public int inverntorySize;
    }
    [Header("Multiple Inventory Try")]
    public InventoryType[] inventories;
    private InventoryType currentInventory;

    public event Action inventoryUpdated;

    private void Awake()
    {
        slots = new InventoryUISlot[inventorySize];
        for (int i = 0; i < inventories.Length; i++)
        {
            inventories[i].slots = new InventoryUISlot[inventories[i].inverntorySize];
        }

    }
    private void Start()
    {
        attackController = GameObject.FindWithTag("Player").GetComponent<AttackController>();
        if (!isInventoryOpen)
        {
            inventoryCanvas.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        //for (int i = 0; i < slots.Length; i++)
        //{
        //    if (slots[i].GetItem() != null)
        //    {
        //        print("Slot num: " + i + " Slot item: " + slots[i].GetItem().name);
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryCanvas.gameObject.SetActive(isInventoryOpen);
            if (isInventoryOpen)
            {
                Time.timeScale = 0.0f;
            }
            else
            {

                Time.timeScale = 1.0f;
            }
        }
        else if (Input.GetKey(KeyCode.Mouse2))
        {

            weaponUI.gameObject.SetActive(true);
            Time.timeScale = 0.0f;

        }
        else if (uiCheck.CheckIsReadyToClose() && !isInventoryOpen)
        {
            
                weaponUI.gameObject.SetActive(false);
                Time.timeScale = 1.0f;
            
        }
        

    }

    public void UseItem(Item item)
    {
        if (item is WeaponConfig w)
        {
            attackController.EquipWeapon(w);
        }
        else if (item is FoodItem f)
        {
            UseFood(f);
        }
    }

    public void DropItem(int index)
    {

        slots[index].item = null;
        if (inventoryUpdated != null)
        {
            inventoryUpdated();
        }
    }
   
    public void UseFood(FoodItem item)
    {
        print("Heal Amount = "+item.healAmount);
    }
    public bool GetInventoryStatus()
    {
        return isInventoryOpen;
    }



    //_-------------------------------------------------------------deneme fonks
    public InventoryUISlot GetSlotInIndex(int index)
    {
        return slots[index];
    }

    public bool AddItemToSlot(int slot, Item item, int number)
    {
        if (slots[slot].item != null)
        {
            return AddToFirstEmptySlot(item, number); ;
        }

        //var i = FindStack(item);
        //if (i >= 0)
        //{
        //    slot = i;
        //}

        slots[slot].item = item;
        if (inventoryUpdated != null)
        {
            inventoryUpdated();
        }
        return true;
    }

    private int FindSlot(Item item)
    {
        //int i = FindStack(item);
        //if (i < 0)
        //{
          int  i = FindEmptySlot();
        //}
        return i;
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {

            if (slots[i].item == null)
            {
                return i;
            }
        }
        return -1;
    }

    public static TempInventory GetPlayerInventory()
    {
        var player = GameObject.FindWithTag("Player");
        return player.GetComponent<TempInventory>();
    }

    public bool HasSpaceFor(Item item)
    {
        return FindSlot(item) >= 0;
    }

    public int GetSize()
    {
        return slots.Length;
    }

    public bool AddToFirstEmptySlot(Item item, int number)
    {
        int i = FindEmptySlot();

        if (i < 0)
        {
            return false;
        }

        slots[i].item = item;
        if (inventoryUpdated != null)
        {
            inventoryUpdated();
        }
        return true;
    }

    public bool HasItem(Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (object.ReferenceEquals(slots[i].item, item))
            {
                return true;
            }
        }
        return false;
    }

    public Item GetItemInSlot(int slot)
    {
        return slots[slot].GetItem();
    }
    public void RemoveFromSlot(int slot, int number)
    {
         
        slots[slot].item = null;
        
        if (inventoryUpdated != null)
        {
            inventoryUpdated();
        }
    }

    
}
