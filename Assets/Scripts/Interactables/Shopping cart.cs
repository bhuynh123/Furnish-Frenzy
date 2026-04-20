using UnityEngine;
public class ShoppingCart : Item
{
    private void Start()
    {
        itemName = "ShoppingCart";
        itemType = "Tool";
        durability = 100;
        needsTwoHandsToPickUp = true;
    }
    public override void useItem()
    {
        Debug.Log("Used " + itemName);
    }
}