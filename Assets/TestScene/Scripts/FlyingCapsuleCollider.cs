using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FlyingCapsuleCollider 
{

    public CapsuleColliderData ColliderData { get; private set; }

    [field: SerializeField] public DefaultColliderData DefaultColliderData { get; private set; }

    [field: SerializeField] public SlopeData SlopeData { get; private set; }

    public void Initilaze(GameObject gameobject)
    {
        if (ColliderData != null)
        {
            return;
        }

        ColliderData = new CapsuleColliderData();

        ColliderData.Initilaze(gameobject);

    }
    public void CalculateCapsuleValues()
    {
        SetCapsuleColliderRadius(DefaultColliderData.Radius);
        SetCapsuleColliderHeight(DefaultColliderData.Height * (1f-SlopeData.StepHeightPercentage));

        RecalculateCapsuleCenter();

        if (ColliderData.Collider.height/2< ColliderData.Collider.radius)
        {
            SetCapsuleColliderRadius(ColliderData.Collider.height / 2);
        }

        ColliderData.UpdateColliderCenter(); 
    }

    private void RecalculateCapsuleCenter()
    {
        float colliderHeightDifference = DefaultColliderData.Height - ColliderData.Collider.height;

        Vector3 newCenter = new(0, DefaultColliderData.CenterY + (colliderHeightDifference / 2), 0);

        ColliderData.Collider.center = newCenter;
    }

    private void SetCapsuleColliderRadius(float radius)
    {
        ColliderData.Collider.radius = radius;
    }
    private void SetCapsuleColliderHeight(float height)
    {
        ColliderData.Collider.height = height;
    }

}
