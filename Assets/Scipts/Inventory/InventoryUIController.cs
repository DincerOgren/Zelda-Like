using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;


public class InventoryUIController : MonoBehaviour
{
    public float xMoveAmount = 200f;
    public float yMoveAmount = 220f;

    public float moveTime = 5f;
    public Transform slideObject;
    public Transform itemsParent;

    public InventoryUsePanelControl usePanel;

    private bool shouldMoveHorizontally = false;
    private bool shouldMoveVertically = false;

    private Vector3 targetPos = Vector3.zero;

    

    public int rowCount=4;
    public int colCount;
    private int colModde;
    //---------
    [Header("UI Changer")]
    public Transform inventoryUIParent;
    public float inventoryMoveAmount = 1050f;
    public float tweenDuration = 1f;
    public int panelCount;
    public int currentPanelCount = 0;

    public Panels[] panelComponents;
    private bool canSwipeUI = true;

    [System.Serializable]
    public struct Panels
    {
        public SlotType type;
        public Transform slideObject;
        public Transform itemsParent;
        public InventoryUsePanelControl usePanel;
    }



    //--------------
    private static readonly float fixedXSumAmount = 650f;
    private static readonly float fixedYSumAmount = -165f;

    private Vector3 curPos;
    private int uiNumber;

    private InventoryUISlot currentSlot = null;
    private int currentCol = 1;

    private TempInventory inventory;

    private bool firstOpen = false;
    private bool canCycleInSlots = true;
    private void Start()
    {
        
        inventory = TempInventory.GetPlayerInventory();
        panelCount = inventoryUIParent.childCount-1;
        colCount = Mathf.CeilToInt(inventory.GetSize()/rowCount);
        colModde = inventory.GetSize() % rowCount;
        //itemCount = itemsParent.childCount;
        //itemList = new InventoryUISlot[Mathf.RoundToInt(itemCount)];
        //FillList(itemList);
        
        //usePanel.dropEvent += FillListAction;
        
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha3) && canSwipeUI)
        {
            

            GoLeft();
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && canSwipeUI)
        {
            //Vector3 pos = slideObject.localPosition;

            GoRight();
            //print(slideObject.localPosition + " Before Pos");
            //slideObject.localPosition = pos;
            //print(slideObject.localPosition + " After pos");
            //print("pos after" + pos);
        }

        if (isActiveAndEnabled && !firstOpen)
        {
            currentSlot = inventory.GetSlotInIndex(0);
            currentSlot.Set();
            targetPos = slideObject.localPosition;
            firstOpen = true;
        }
        //else
        //    firstOpen = false;

        if (!usePanel.UsePanelStatus() && canCycleInSlots)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (currentSlot != null)
                    currentSlot.DeActivate();
                targetPos.x = Mathf.Min(targetPos.x + xMoveAmount, (((rowCount - 1) * xMoveAmount) - fixedXSumAmount));
                shouldMoveHorizontally = true;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                if (currentSlot != null)
                    currentSlot.DeActivate();
                targetPos.x = MathF.Max(targetPos.x - xMoveAmount, 0 - fixedXSumAmount);
                shouldMoveHorizontally = true;
            }

            if (Input.GetKeyDown(KeyCode.S) )
            {
                currentCol = Mathf.Min(currentCol + 1, colCount);
                if (currentSlot != null)
                    currentSlot.DeActivate();
                targetPos.y = MathF.Max(targetPos.y - yMoveAmount, ((colCount - 1) * -yMoveAmount)-fixedYSumAmount);
                shouldMoveVertically = true;
            }
            if (Input.GetKeyDown(KeyCode.W) )
            {
                currentCol = Mathf.Max(currentCol - 1, 1);
                if (currentSlot != null)
                    currentSlot.DeActivate();
                targetPos.y = MathF.Min(targetPos.y + yMoveAmount, 0 - fixedYSumAmount);
                shouldMoveVertically = true;
            }
        }
        if (shouldMoveVertically || shouldMoveHorizontally)
        {
            if (shouldMoveHorizontally)
            {
                MoveUI();
                curPos = slideObject.localPosition;
                curPos.x += fixedXSumAmount;
                currentSlot = ChooseObject(curPos.x);
                currentSlot.Set();
                shouldMoveHorizontally = false;
            }
            if (shouldMoveVertically)
            {
                MoveUI();

                curPos = slideObject.localPosition;
                curPos.x += fixedXSumAmount;
                currentSlot = ChooseObject(curPos.x);

                currentSlot.Set();
                shouldMoveVertically = false;
            }
        }


    }

    private InventoryUISlot ChooseObject(float value)
    {
        uiNumber = Mathf.Abs(Mathf.RoundToInt(value / xMoveAmount));
        //return itemsParent.GetChild(uiNumber + ((currentCol - 1) * rowCount)).GetComponent<InventoryUISlot>();
        return inventory.GetSlotInIndex(uiNumber + ((currentCol - 1) * rowCount));
    }

    public InventoryUISlot GetCurrentSlot()
    {
        return currentSlot;
    }

  
    //private void FillList(InventoryUISlot[] item)
    //{
    //    for (int i = 0; i < item.Length; i++)
    //    {
    //        item[i] = itemsParent.GetChild(i).GetComponent<InventoryUISlot>();
    //    }
    //}

    public bool CheckIsReadyToClose()
    {
        return Vector3.Distance(slideObject.localPosition, targetPos) <= .5f;
    }

    private void MoveUI()
    {

        slideObject.localPosition = targetPos;

    }


    private void GoLeft()
    {
        if (currentPanelCount==panelCount)
        {
            return;
        }
        canSwipeUI = false;
        
        canCycleInSlots = false;
        currentPanelCount++;
        Vector3 pos = slideObject.localPosition;
        slideObject.gameObject.SetActive(false);
        inventoryUIParent.DOMoveX(inventoryUIParent.position.x - inventoryMoveAmount, tweenDuration).SetUpdate(true)
            .OnComplete(()=> { 
                canCycleInSlots = true; 
                canSwipeUI = true;
                SetTransforms(currentPanelCount);
                slideObject.localPosition = pos;
                slideObject.gameObject.SetActive(true);

            });
        

    }
    private void GoRight()  
    {
        if (currentPanelCount == 0)
        {
            return;
        }
        canSwipeUI = false;
        canCycleInSlots = false;
        currentPanelCount--;
        Vector3 pos = slideObject.localPosition;
        slideObject.gameObject.SetActive(false);

        inventoryUIParent.DOMoveX(inventoryUIParent.position.x + inventoryMoveAmount, tweenDuration).SetUpdate(true)
            .OnComplete(() => {
                canCycleInSlots = true;
                canSwipeUI = true;
                SetTransforms(currentPanelCount);
                slideObject.localPosition = pos;
                slideObject.gameObject.SetActive(true);

            });
    }

    private void SetTransforms(int index)
    {
        slideObject = panelComponents[index].slideObject;
        itemsParent = panelComponents[index].itemsParent;
        usePanel = panelComponents[index].usePanel;
    }
}


