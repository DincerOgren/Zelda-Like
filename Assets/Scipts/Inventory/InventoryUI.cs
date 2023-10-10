using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] InventoryUISlot InventoryItemPrefab = null;
    [SerializeField] InventoryUsePanelControl usePanel = null;
    [SerializeField] InventoryUIController mainControl;
    [SerializeField] SlotType slotType;
    [SerializeField] int panelIndex;
    TempInventory playerInventory;
   
    private bool isFinished1,isFinished2;
    private void Awake()
    {
        playerInventory = TempInventory.GetPlayerInventory();
        playerInventory.inventoryUpdated += Redraw;
    }

    private void Start()
    {
        slotType = mainControl.panelComponents[panelIndex].type;
        usePanel = mainControl.panelComponents[panelIndex].usePanel;

        Spawn();
            
        Redraw();
    }

    private void Spawn()
    {
        for (int i = 0; i < playerInventory.GetEverySize(slotType); i++)
        {
            Instantiate(InventoryItemPrefab,transform);
        }
        isFinished1 = true;
    }

    private void Redraw()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }


        for (int i = 0; i < playerInventory.GetEverySize(slotType); i++)
        {
            var itemUI=transform.GetChild(i).GetComponent<InventoryUISlot>();
            itemUI.Setup(playerInventory, i,usePanel,slotType);
            itemUI.gameObject.SetActive(true);
            //var itemUI = Instantiate(InventoryItemPrefab, transform);
        }
        isFinished2 = true;
    }

    public bool FinishedLoading()
    {
        return (isFinished1 && isFinished2);
    }
}
