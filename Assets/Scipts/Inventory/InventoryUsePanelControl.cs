using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUsePanelControl : MonoBehaviour
{
    TempInventory inventory;
    public InventoryUIController mainControl;
    public InventoryUISlot currentSlot;
    public Item curItem = null;
    public TextMeshProUGUI useText;

    
    private bool isActive = false;
    private bool isItemSelected = false;



    [Header("For Choose Variables")]
    public float moveAmount = 65f;
    public Transform chooseObject;


    private Vector3 targetPos;
    private Vector3 curPos;
    private Vector3 startPos;

    public int colCount = 3;
    private int currentCol = 0;
    private bool canSelect = false;
    private static string weaponText = "Equip";
    private static string foodText = "Use";
    private float fixedMovePos = 65f;

    public event Action dropEvent;
    private void Awake()
    {
        inventory = GameObject.FindWithTag("Player").GetComponent<TempInventory>();
    }
    private void Start()
    {
        startPos = chooseObject.localPosition;
    }
    private void Update()
    {

        if (isActive && !isItemSelected)
        {
            currentSlot = mainControl.GetCurrentSlot();
            curItem = currentSlot.GetItem();
            
            if (curItem != null) 
            {
                isItemSelected = true;
                curPos = chooseObject.localPosition;
                targetPos = curPos;
                canSelect = true;
            }
            else { isItemSelected = false; }

        }

        if (isActive && isItemSelected)
        {
                        
            if (Input.GetKeyDown(KeyCode.S) )
            {
                currentCol = Mathf.Min(currentCol + 1,colCount-1);
                targetPos.y = Mathf.Max(targetPos.y-moveAmount,((colCount-1)*-moveAmount)+fixedMovePos);
                chooseObject.localPosition = targetPos;
            }
            if (Input.GetKeyDown(KeyCode.W) )
            {
                currentCol = Mathf.Max(currentCol - 1, 0);
                targetPos.y = Mathf.Min(targetPos.y + moveAmount, 0 + fixedMovePos);

                chooseObject.localPosition = targetPos;
            }

            if (canSelect)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    switch (currentCol)
                    {
                        case 0:
                            inventory.UseItem(curItem);
                            canSelect = false;
                            isActive = false;
                            gameObject.SetActive(false);
                            break;
                        case 1:
                            inventory.DropItem(currentSlot.GetIndex());
                            if (dropEvent!=null)
                            {
                                dropEvent();
                            }
                            isActive = false;
                            canSelect = false;
                            gameObject.SetActive(false);
                            break;
                        case 2:
                            isActive = false;

                            canSelect = false;
                            gameObject.SetActive(false);
                            break;
                        default:
                            break;
                    }

                }
            }

        }

        if (!isActive)
        {
            isItemSelected = false;
            currentCol = 0;
            chooseObject.localPosition = startPos;
        }
    }

    public void SetActive()
    {
        isActive = true;
        currentSlot = mainControl.GetCurrentSlot();
        curItem = currentSlot.GetItem();

        if (curItem != null)
        {
            if (curItem.GetItemType() == ItemType.Weapon)
            {
                useText.text = weaponText;
            }
            if (curItem.GetItemType() == ItemType.Food)
            {
                useText.text = foodText;
            }
        }
    }
    public bool UsePanelStatus()
    {
        return isActive;
    }
    public InventoryUISlot GetActiveSlot()
    {
        return currentSlot;
    }
}
