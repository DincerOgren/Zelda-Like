using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : ScriptableObject
{
    [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
    [SerializeField] string itemID = null;
    [SerializeField] string displayName = null;
    [Tooltip("Item description to be displayed in UI.")]
    [SerializeField][TextArea] string description = null;
    [Tooltip("The UI icon to represent this item in the inventory.")]
    [SerializeField] Sprite icon = null;
    [Tooltip("The UI icon to represent this item in the inventory.")]
    [SerializeField] SlotType itemType;
    [SerializeField] Pickup pickup;

    public Sprite GetIcon()
    {
        return icon;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    public Pickup SpawnPickup(Vector3 position, int number)
    {
        var pickup = Instantiate(this.pickup);
        pickup.transform.position = position;
        pickup.Setup(this, number);
        return pickup;
    }
    public string GetDescription()
    {
        return description;
    }

    public SlotType GetItemType()
    {
        return itemType;
    }

    public virtual void Use()
    {

    }
    public virtual void Drop()
    {

    }

}
