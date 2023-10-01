using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCountTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print(transform.childCount);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            print("child " + i +" name");
        }

        List<int> list = new List<int>();
        list.Add(0);
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Add(4);

        print("Capacity" + list.Capacity + " Count " + list.Count);

        list.Remove(0);
        print("After Remove");
        print("Capacity" + list.Capacity + " Count " + list.Count);


        //for (int i = 0; i < list.Capacity; i++)
        //{
        //    print(list[i]);
        //}
    }
}
