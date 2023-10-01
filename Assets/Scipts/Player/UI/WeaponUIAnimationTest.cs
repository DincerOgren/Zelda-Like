using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUIAnimationTest : MonoBehaviour
{
    public Animator anim;

    private int whichSlot=0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Time.timeScale = 0.0f;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Time.timeScale = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            anim.SetTrigger("0"+"r");
        }
    }

    public void Check()
    {
        Time.timeScale = 1f;
    }
}
