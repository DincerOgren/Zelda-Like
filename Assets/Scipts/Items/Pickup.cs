using UnityEngine;
/// <summary>
/// To be placed at the root of a Pickup prefab. Contains the data about the
/// pickup such as the type of item and the number.
/// </summary>
public class Pickup : MonoBehaviour
{
    // STATE
    Item item;
    int number = 1;

    // CACHED REFERENCE
    TempInventory inventory;

    // LIFECYCLE METHODS

    private void Awake()
    {

        inventory = TempInventory.GetPlayerInventory(); ;
    }

    // PUBLIC

    /// <summary>
    /// Set the vital data after creating the prefab.
    /// </summary>
    /// <param name="item">The type of item this prefab represents.</param>
    /// <param name="number">The number of items represented.</param>
    public void Setup(Item item, int number)
    {
        this.item = item;
       
    }

    public Item GetItem()
    {
        return item;
    }

    public int GetNumber()
    {
        return number;
    }

    public void PickupItem()
    {
        bool foundSlot = inventory.AddToFirstEmptySlot(item, number,item.GetItemType());
        if (foundSlot)
        {
            print("Added to slot " + item.GetItemType());
            Destroy(gameObject);
        }
    }

    public bool CanBePickedUp()
    {
        return inventory.HasSpaceFor(item);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickupItem();
        }
    }
}