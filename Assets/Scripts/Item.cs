using UnityEngine;

public abstract class Item : Interactable
{
    public string itemName;
    public Sprite icon;
    protected string itemType;
    protected float durability;
    public bool needsTwoHandsToPickUp;

    public HeldItemPhysics physicsController; // Add this!

    private void Awake()
    {
        physicsController = GetComponent<HeldItemPhysics>();
    }

    public abstract void useItem();

    protected override void Interact()
    {
        if (Inventory.instance.AddItem(this))
        {
            // Do NOT parent the item. 
            // Equip it immediately (this disables other items and enables this one)
            Inventory.instance.EquipItem();
        }
    }
}