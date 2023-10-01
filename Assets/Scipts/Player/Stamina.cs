using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    [SerializeField]
    private float stamina = 30f;

    [SerializeField]
    private float decreaseAm = 20f;

    [SerializeField]
    private float incAm = 20f,tiredIncAm=10f;

    [SerializeField]
    private CanvasGroup rootCanva;
    [SerializeField]
    private float alphaReduce = 5f;



    [SerializeField]
    private Image greenCircle;
    [SerializeField]
    private Image redIndicator;

    private float curStamina;

    [Header("COLOR TRANSITION FOR CIRCLE")]


    public Color redColor = Color.red;
    public Color orangeishColor = new(1.0f, 0.5f, 0.0f); 
    public float colorTransitionSpeed = 10f; 
    private Color greenColor = new(0, 1f, 0.25f);
    private float time;
    private Quaternion startingRotation;

    [Header("COLOR TRANSITION FOR CIRCLE")]
    public Color yellow = new(1.0f, 0.5f, 0.0f);
    private void Start()
    {
        curStamina = stamina;
        startingRotation = redIndicator.rectTransform.localRotation;
    }
    private void Update()
    {
        UpdateStaminaUI();
        if (IsStaminaFull())
        {
            greenCircle.color = greenColor;
            rootCanva.alpha -= alphaReduce * Time.deltaTime;
        }
        else
        {
            rootCanva.alpha = 1;
        }
    }


    //----------STAMINA UI FUNCTIONS-------------
    private void UpdateStaminaUI()
    {
        float fillAmount = curStamina / stamina;

        greenCircle.fillAmount = fillAmount;

        float indicatorAngle = 360.0f * (1.0f - fillAmount);
        Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, -indicatorAngle) * startingRotation;

        redIndicator.rectTransform.localRotation = targetRotation;
    }

    private void ColorTransition(Image image,Color first,Color second)
    {

        float colorTransitionValue = Mathf.Sin(time * colorTransitionSpeed);

        Color lerpedColor = Color.Lerp(first, second, (colorTransitionValue + 1.0f) / 2.0f);
        image.color = lerpedColor;

        time += Time.deltaTime;

    }




    //------------STAMINA CHECKS-----------
    public bool IsStaminaFull()
    {
        return curStamina == stamina;    
    }
    public void DecreaseStamina(float decraseAmount)
    {
        redIndicator.gameObject.SetActive(true);
        ColorTransition(redIndicator,redColor,yellow);
        curStamina -= decraseAmount * Time.deltaTime;
    }
    public void IncreaseStamina(float incAmount)
    {
        redIndicator.gameObject.SetActive(false);
        curStamina += incAmount * Time.deltaTime;
        float temp = Mathf.Min(curStamina, stamina);
        curStamina = temp;
    }
    public void TiredStaminaIncrease(float incAmount)
    {
        redIndicator.gameObject.SetActive(false);
        ColorTransition(greenCircle,redColor,orangeishColor);
        curStamina += incAmount * Time.deltaTime;
        float temp = Mathf.Min(curStamina, stamina);
        curStamina = temp;
    }





    //---------GETTERS---------
    public float GetStamina()
    {
        return curStamina;
    }


    public float GetIncAmount()
    {
        return incAm;
    }

    public float GetTiredIncAmount()
    {
        return tiredIncAm;
    }
    public float GetDAmount()
    {
        return decreaseAm;
    }

  
}
