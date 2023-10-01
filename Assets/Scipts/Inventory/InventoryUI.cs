using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] InventoryUISlot InventoryItemPrefab = null;
    [SerializeField] InventoryUsePanelControl usePanel = null;

    TempInventory playerInventory;
   

    private void Awake()
    {
        playerInventory = TempInventory.GetPlayerInventory();
        playerInventory.inventoryUpdated += Redraw;
        
    }

    private void Start()
    {
        Spawn();
            
        Redraw();
    }

    private void Spawn()
    {
        for (int i = 0; i < playerInventory.GetSize(); i++)
        {
            Instantiate(InventoryItemPrefab,transform);
        }
    }

    private void Redraw()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }


        for (int i = 0; i < playerInventory.GetSize(); i++)
        {
            var itemUI=transform.GetChild(i).GetComponent<InventoryUISlot>();
            itemUI.Setup(playerInventory, i,usePanel);
            itemUI.gameObject.SetActive(true);
            //var itemUI = Instantiate(InventoryItemPrefab, transform);
        }
    }

    
}
