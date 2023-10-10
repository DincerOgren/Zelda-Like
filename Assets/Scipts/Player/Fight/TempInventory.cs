using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class TempInventory : MonoBehaviour
{
    public Canvas weaponUI;
    public Canvas inventoryCanvas;
    public WeaponUIController uiCheck;
    private bool isInventoryOpen = false;

    [Header("Inventory Size")]

    private AttackController attackController;


    [Serializable]
    public struct InventoryType
    {
        public SlotType inventoryType;
        public InventoryUISlot[] slots;
        public int inventorySize;
    }
    [Header("Multiple Inventory Try")]
    public InventoryType[] inventories;
    private InventoryType currentInventory;

    public event Action inventoryUpdated;
    public event Action updateWeapons;

    PlayerInputTesting playerInputs;

    private bool shouldOpen = false;


    [Header("New Input System Values")]
    private bool tabKey, middleMouseButton;

    [Header("For Dropping Items")]
    public Transform dropPoint;
    private void Awake()
    {
        for (int i = 0; i < inventories.Length; i++)
        {
            inventories[i].slots = new InventoryUISlot[inventories[i].inventorySize];
        }
        playerInputs = new PlayerInputTesting();

    }
    private void OnEnable()
    {
        playerInputs.InventoryUIKeys.Enable();
    }
    private void OnDisable()
    {
        playerInputs.InventoryUIKeys.Disable();
    }
    private void Start()
    {
        attackController = GameObject.FindWithTag("Player").GetComponent<AttackController>();
        if (!isInventoryOpen)
        {
        }
        SetInputEvents();
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
        if (tabKey)
        {
            tabKey = false;
            isInventoryOpen = !isInventoryOpen;
            inventoryCanvas.gameObject.SetActive(isInventoryOpen);
            if (isInventoryOpen)
            {
                Time.timeScale = 0.0f;
                playerInputs.CharacterMovement.Disable();
            }
            else
            {
                playerInputs.CharacterMovement.Enable();
                Time.timeScale = 1.0f;
            }
        }
        else if (middleMouseButton)
        {
            middleMouseButton = false;
            shouldOpen = !shouldOpen;
            weaponUI.gameObject.SetActive(shouldOpen);
            //Time.timeScale = 0.0f;

            if (shouldOpen)
            {
                playerInputs.CharacterMovement.Disable();
                Time.timeScale = 0.0f;
            }
            else
            {
                playerInputs.CharacterMovement.Enable();
                Time.timeScale = 1.0f;
            }

        }
        else if (uiCheck.CheckIsReadyToClose() && !isInventoryOpen)
        {

           // weaponUI.gameObject.SetActive(false);
            //Time.timeScale = 1.0f;

        }


    }

    private void SetInputEvents()
    {
        playerInputs.InventoryUIKeys.TabInventory.started += TabPressed;
        playerInputs.InventoryUIKeys.WeaponUI.started += MousePressed;
    }

    private void TabPressed(InputAction.CallbackContext context)
    {
        tabKey = !tabKey;
    }

    private void MousePressed(InputAction.CallbackContext context)
    {
        middleMouseButton = !middleMouseButton;
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

    public void DropItem(int index,SlotType type)
    {
        var selectedInventory=GetInventoryFromType(type);
        var item = selectedInventory.slots[index].item;
        selectedInventory.slots[index].item = null;

        var pikcup=item.SpawnPickup(dropPoint.position, 1);
        pikcup.GetComponent<Rigidbody>().isKinematic = false;
        pikcup.GetComponent<Rigidbody>().useGravity = true;
        //Instantiate(item, dropPoint.position, Quaternion.identity);
        if (inventoryUpdated != null)
        {
            inventoryUpdated();
        }
    }

    public void UseFood(FoodItem item)
    {
        print("Heal Amount = " + item.healAmount);
    }
    public bool GetInventoryStatus()
    {
        return isInventoryOpen;
    }



    //_-------------------------------------------------------------deneme fonks
    public InventoryUISlot GetSlotInIndex(int index,SlotType type)
    {
        foreach (var item in inventories) 
        {
            if (item.inventoryType == type)
            {
                return item.slots[index];
            }
        }

        return null;
    }

    //public bool AddItemToSlot(int slot, Item item, int number)
    //{
    //    if (slots[slot].item != null)
    //    {
    //        return AddToFirstEmptySlot(item, number); ;
    //    }

    //    //var i = FindStack(item);
    //    //if (i >= 0)
    //    //{
    //    //    slot = i;
    //    //}

    //    slots[slot].item = item;
    //    if (inventoryUpdated != null)
    //    {
    //        inventoryUpdated();
    //    }
    //    return true;
    //}

    private int FindSlot(Item item)
    {
        //int i = FindStack(item);
        //if (i < 0)
        //{
        int i = FindEmptySlot(item.GetItemType());
        //}
        return i;
    }
    public int GetFilledAmount(SlotType type)
    {
        var inventorie = GetInventoryFromType(type);
        int amount = 0;
        for (int i = 0; i < inventorie.inventorySize; i++)
        {
            if (inventorie.slots[i].item == null)
            {
                continue;
            }
            else
                amount++;
        }

        return amount;
    }
    private int FindEmptySlot(SlotType type)
    {

        foreach (var item in inventories)
        {
            if (item.inventoryType == type)
            {
                for (int i = 0; i < item.slots.Length; i++)
                {

                    if (item.slots[i].item == null)
                    {
                        return i;
                    }
                }
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

    public int GetEverySize(SlotType type)
    {

        foreach (var slot in inventories)
        {
            if (slot.inventoryType == type)
            {
                return slot.inventorySize;
            }
        }

        print("HATA SUNDA +=" + type.ToString());
        return -1;
        //return inventories[n].inventorySize;
    }

    
    //public int GetSize()
    //{
    //    return slots.Length;
    //}

    public bool AddToFirstEmptySlot(Item item, int number, SlotType type)
    {
        int i = FindEmptySlot(type);

        if (i < 0)
        {
            return false;
        }

        foreach (var inventory in inventories)
        {
            if (inventory.inventoryType == type)
            {
                inventory.slots[i].item = item;
                print("try to ad to this type= " + inventory.inventoryType.ToString());
            }
        }
        if (inventoryUpdated != null)
        {
            inventoryUpdated();
        }
        return true;
    }

    //public bool HasItem(Item item)
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        if (object.ReferenceEquals(slots[i].item, item))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    public Item GetItemInSlot(int slot,SlotType type)
    {
        foreach (var item in inventories)
        {
            if (item.inventoryType == type)
            {
                return item.slots[slot].item;
            }
        }

        return null;
    }
    //public void RemoveFromSlot(int slot, int number)
    //{

    //    slots[slot].item = null;

    //    if (inventoryUpdated != null)
    //    {
    //        inventoryUpdated();
    //    }
    //}

    public InventoryType GetInventoryFromType(SlotType type)
    {
        foreach (var item in inventories)
        {
            if (item.inventoryType == type)
            {
                return item;
            }
        }

        print("ERROR BOS TYPE");
        InventoryType i=new();
        return i;
    }
}
