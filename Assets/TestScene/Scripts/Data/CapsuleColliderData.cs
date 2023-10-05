using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleColliderData
{
    public CapsuleCollider Collider { get; private set; }

    public Vector3 ColliderCenterInLocalSpace { get; private set; }

    public void Initilaze(GameObject gameobject)
    {
        if (Collider != null)
        {
            return;
        }

        Collider=gameobject.GetComponent<CapsuleCollider>();

        UpdateColliderCenter();
    }

    public void UpdateColliderCenter()
    {
        ColliderCenterInLocalSpace = Collider.center;
    }
}
