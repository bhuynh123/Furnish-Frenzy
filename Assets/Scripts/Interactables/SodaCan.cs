using UnityEngine;
public class SodaCan : Item
{
    private void Start()
    {
        itemName = "Soda Can";
        itemType = "PowerUp";
        durability = 1;
        needsTwoHandsToPickUp = false;
    }
    public override void useItem()
    {
        Debug.Log("Used " + itemName);
    }
}