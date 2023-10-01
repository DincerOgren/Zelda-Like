
using UnityEngine;

[CreateAssetMenu(menuName ="Create Food/Empty")]
public class FoodItem : Item
{
    public float healAmount;

    public override void Use()
    {
        base.Use();
        Debug.Log("FOOD ITEM NAME: ");
    }
    public override void Drop()
    {
        base.Drop();
        Debug.Log("FOOD ITM DROP: : : :");
    }
}
