using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class WeaponUIController : MonoBehaviour
{
    public Canvas ui;
    public float moveAmount = 280;
    public float moveTime = 5f;
    public Transform slideObject;


    private bool shouldMoveRight = false;
    private bool shouldMoveLeftt = false;
    private Vector3 targetPos = Vector3.zero;
    private Transform itemParent;

    private float itemCount;

    private Vector3 curPos;
    private int uiNumber;

    private MeleeCombatUI chosenItem;

    private void Start()
    {
        itemParent = slideObject.GetChild(0);
        itemCount = itemParent.childCount - 1;
        chosenItem = ChooseObject(curPos.x);
        chosenItem.Set();

    }
    private void Update()
    {



        if (!ui.gameObject.activeSelf)
            return;

        if (shouldMoveRight || shouldMoveLeftt)
        {
            if (shouldMoveLeftt)
            {
                MoveUI(false);
                if (Vector3.Distance(slideObject.localPosition, targetPos) <= 3f)
                {
                    slideObject.localPosition = targetPos;
                    curPos = slideObject.localPosition;
                    chosenItem = ChooseObject(curPos.x);

                    chosenItem.Set();

                    shouldMoveLeftt = false;
                }
            }
            if (shouldMoveRight)
            {
                MoveUI(true);
                if (Vector3.Distance(slideObject.localPosition, targetPos) <= 3f)
                {
                    slideObject.localPosition = targetPos;
                    curPos = slideObject.localPosition;
                    chosenItem = ChooseObject(curPos.x);

                    chosenItem.Set();
                    shouldMoveRight = false;
                }
            }
        }
        else
        {
            print("UINUMBER: -----" + uiNumber);


            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                //print("anim" + uiNumber + right);
                //anim.SetTrigger(uiNumber + right);
                chosenItem.Drop();
                targetPos = slideObject.localPosition;
                targetPos.x = Mathf.Max(targetPos.x - moveAmount, itemCount * -moveAmount);
                shouldMoveLeftt = true;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //    print("anim" + uiNumber + left);

                //    anim.SetTrigger(uiNumber + left);
                chosenItem.Drop();
                shouldMoveRight = true;
                targetPos = slideObject.localPosition;
                targetPos.x = MathF.Min(targetPos.x + moveAmount, 0);
            }
        }




    }

    private MeleeCombatUI ChooseObject(float value)
    {
        uiNumber = Mathf.Abs(Mathf.RoundToInt(value / moveAmount));
        return itemParent.GetChild(uiNumber).GetComponent<MeleeCombatUI>();
    }

    public bool CheckIsReadyToClose()
    {
        return Vector3.Distance(slideObject.localPosition, targetPos) <= .5f;
    }

    private void MoveUI(bool value)
    {
        if (!value)
        {
            StartCoroutine(Slide());
        }

        if (value)
        {
            StartCoroutine(Slide());
        }

    }

    private IEnumerator Slide()
    {
        slideObject.localPosition = Vector3.Lerp(slideObject.localPosition, targetPos, moveTime * Time.unscaledDeltaTime);
        yield return null;
    }
}
