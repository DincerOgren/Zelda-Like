using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCountTest : MonoBehaviour
{
    // Start is called before the first frame update
    public int a;
    public int b;

    public int intResult;


    [Header("Float")]

    public float ab;
    public float bb;
    public float floatResult;

    public int floatboluint;
    public float floatboluintFloat;


    private void Update()
    {
        intResult = a / b;

        floatResult= ab / bb;

       // floatboluint = ab / b;
        floatboluintFloat = a / bb;

    }
}
