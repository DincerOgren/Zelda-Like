using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUISlot : MonoBehaviour
{
    public bool isChosen = false;
    private bool isEquipped = false;
    public InventoryUsePanelControl usePanel;
    public Vector3 panelOffset;
    public Image itemIcon;

    public Item item=null;


    private TempInventory inventory;
    private SlotType inventoryType;
    private int index;
    private Vector3 curPos=Vector3.zero;



    public void Setup(TempInventory inventory, int index, InventoryUsePanelControl usePanel,SlotType type)
    {
        this.inventory=inventory;
        this.index=index;
        this.usePanel=usePanel;
        this.inventoryType = type;
        if (inventory.GetSlotInIndex(index,inventoryType) == null)
        {
            foreach (var inventorie in inventory.inventories)
            {
                if (inventorie.inventoryType == inventoryType)
                {
                    inventorie.slots[index]=this;
                }
            }

        }
        else
        {
            item = inventory.GetItemInSlot(index,inventoryType);
        }


        if (inventory.GetItemInSlot(index,inventoryType) != null)
        {
            itemIcon.sprite = item.GetIcon();
            itemIcon.gameObject.SetActive(true);

        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.gameObject.SetActive(false);
        }
    }


    private void Start()
    {
        

    }

    private void Update()
    {
        
        if (isChosen && !isEquipped)
        {
            isEquipped = true;
        }

        if (isEquipped)
        {
            if (Input.GetKeyDown(KeyCode.E) && item!=null)
            {
                curPos = transform.position;
                curPos += panelOffset;
                usePanel.gameObject.SetActive(true);
                itemIcon.sprite=item.GetIcon();
                usePanel.SetActive();
                usePanel.transform.position = curPos;
                itemIcon.gameObject.SetActive(true);
            }

        }
    }
    public void Set()
    {
        isChosen = true;
    }
    public void DeActivate()
    {
        isChosen = false;
        isEquipped = false;
    }

    public void DropItem()
    {
        //item.SpawnPickup();
        item = null;

    }
    public Item GetItem()
    {
        //if (item == null)
        //{
        //    print("item null ifi");
        //}
        //if (item != null)
        //{
        //    print("item null != ifi");
        //}

        return item;
    }

    public SlotType GetSlotType()
    {
        return inventoryType;
    }
    public int GetIndex()
    {
        return index;
    }
}

