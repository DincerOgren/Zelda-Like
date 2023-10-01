using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{

    // CONFIG DATA
   public Item item = null;
    [SerializeField] int number = 1;

    // LIFECYCLE METHODS
    private void Awake()
    {
        
        SpawnPickup();
    }


    public Pickup GetPickup()
    {
        return GetComponentInChildren<Pickup>();
    }

    /// <summary>
    /// True if the pickup was collected.
    /// </summary>
    public bool isCollected()
    {
        return GetPickup() == null;
    }

    //PRIVATE

    private void SpawnPickup()
    {
        var spawnedPickup = item.SpawnPickup(transform.position, number);
        spawnedPickup.transform.SetParent(transform);
    }
    
}
