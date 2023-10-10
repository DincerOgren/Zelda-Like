using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public InventoryUI[] uis;
    public float amount=.2f;
    public float LoadTime=1f;
    public Canvas weaponUI;
    public Canvas inventoryCanvas;
    CanvasGroup group;
    private Coroutine currentActiveFade;

    private bool loop=false;
    // Start is call
    // ed before the first frame update
    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float sum = 0;
        while(!loop)
        {
            print("hala icerde");
            foreach(var u in uis)
            {
                if (u.FinishedLoading())
                {
                    sum++;
                }

            }

            if (sum == uis.Length)
            {
                FadeOut(LoadTime);
                inventoryCanvas.gameObject.SetActive(false);
                weaponUI.gameObject.SetActive(false);
                loop = true;
                break;
            }
            else
            {
                sum = 0;
            }
        }
    }

    public Coroutine FadeOut(float time)
    {
        return Fade(0, time);
        print("Bitti");
    }

    private IEnumerator FadeRoutine(float target, float time)
    {
        while (!Mathf.Approximately(group.alpha, target))
        {
            group.alpha = Mathf.MoveTowards(group.alpha, target, Time.deltaTime / time);
            yield return null;
        }
        print("Bitti");

    }
    public Coroutine FadeIn(float time)
    {
        return Fade(1, time);

    }

    Coroutine Fade(float target, float time)
    {
        if (currentActiveFade != null)
        {
            StopCoroutine(currentActiveFade);
        }
        currentActiveFade = StartCoroutine(FadeRoutine(target, time));
        return currentActiveFade;
    }

    IEnumerator LoadGame()
    {
        group.alpha -= amount * Time.time;
        yield return new WaitForSeconds(LoadTime);

        //inventoryCanvas.gameObject.SetActive(false);
        //weaponUI.gameObject.SetActive(false);
       
    }
}
