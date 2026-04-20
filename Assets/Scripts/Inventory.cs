using UnityEngine;
using UnityEngine.UI;
using System;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    private readonly static ushort LIST_CAPACITY = 5;

    // CHANGED: Using a fixed array instead of a List to allow for 'null' empty slots
    public Item[] items;

    [SerializeField]
    private ushort selectedItem = 0;
    private GameObject[] slotIcons;
    public Transform dropPoint;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Debug.LogWarning("More than one instance of inventory found!");

        // --- THE FIX: Force the array to be exactly 5, ignoring the Inspector ---
        items = new Item[LIST_CAPACITY];

        slotIcons = new GameObject[LIST_CAPACITY];
        for (int i = 0; i < LIST_CAPACITY; i++)
        {
            slotIcons[i] = GameObject.Find("/Canvas/Inventory").transform.GetChild(i).gameObject;
        }
        UpdateUI();
    }

    // --- TWO-HANDED LOCK LOGIC ---
    private bool IsHoldingTwoHandedItem()
    {
        if (items[selectedItem] != null)
        {
            return items[selectedItem].needsTwoHandsToPickUp;
        }
        return false;
    }

    // --- ITEM MANAGEMENT ---
    public bool AddItem(Item newItem)
    {
        // Find the first empty (null) slot in the array
        for (ushort i = 0; i < LIST_CAPACITY; i++)
        {
            if (items[i] == null)
            {
                items[i] = newItem;
                Debug.Log("Added " + newItem.itemName + " to slot " + i);
                UpdateUI();
                return true;
            }
        }

        Debug.Log("Inventory is full!");
        return false;
    }

    public void DropItem()
    {
        if (items[selectedItem] == null) return; // Can't drop empty air

        try
        {
            Item itemToDrop = items[selectedItem];

            itemToDrop.physicsController.OnUnequip();
            itemToDrop.transform.parent = null;
            itemToDrop.transform.position = dropPoint != null ? dropPoint.position : transform.position + (transform.forward * 2);
            itemToDrop.gameObject.SetActive(true);

            // CHANGED: Set the slot to null instead of removing it from a list
            items[selectedItem] = null;

            UpdateUI();
            EquipItem(); // Will equip "nothing" since the slot is now null
        }
        catch (Exception ex)
        {
            Debug.LogError("Error dropping item: " + ex.Message);
        }
    }

    public void UseItem()
    {
        if (items[selectedItem] != null)
        {
            items[selectedItem].useItem();
            Destroy(items[selectedItem].gameObject);
            items[selectedItem] = null; // Clear the slot
            UpdateUI();
            EquipItem();
        }
    }

    public void EquipItem()
    {
        // 1. Turn off and unequip ALL items
        for (ushort i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                items[i].physicsController.OnUnequip();
                items[i].gameObject.SetActive(false);
            }
        }

        // 2. Turn on and equip ONLY the selected item (if it exists)
        if (items[selectedItem] != null)
        {

            Item currentItem = items[selectedItem];
            currentItem.transform.parent = null;
            currentItem.gameObject.SetActive(true);
            currentItem.transform.position = dropPoint.position;
            currentItem.physicsController.OnEquip();
        }
    }

    private void UpdateUI()
    {
        for (ushort i = 0; i < slotIcons.Length; i++)
        {
            if (items[i] != null)
            {
                Image img = slotIcons[i].GetComponent<Image>();
                img.sprite = items[i].icon;
                slotIcons[i].SetActive(true);
            }
            else
            {
                slotIcons[i].SetActive(false); // Hide icon if slot is empty
            }
        }
    }

    // --- SELECTION & INPUT LOGIC ---

    // Centralized method to handle slot changes so we don't repeat the two-handed check
    private void ChangeSelectedSlot(ushort newSlot)
    {
        if (IsHoldingTwoHandedItem())
        {
            Debug.Log("Hands are full! You must drop the heavy item first.");
            return; // Block the switch
        }

        selectedItem = newSlot;
        UpdateUI();
        EquipItem();
    }

    public void item1Select() { ChangeSelectedSlot(0); }
    public void item2Select() { ChangeSelectedSlot(1); }
    public void item3Select() { ChangeSelectedSlot(2); }
    public void item4Select() { ChangeSelectedSlot(3); }
    public void item5Select() { ChangeSelectedSlot(4); }

    public void scrollUp()
    {
        if (IsHoldingTwoHandedItem()) return; // Block scroll

        selectedItem = (ushort)((selectedItem < LIST_CAPACITY - 1) ? selectedItem + 1 : 0);
        UpdateUI();
        EquipItem();
    }

    public void scrollDown()
    {
        if (IsHoldingTwoHandedItem()) return; // Block scroll

        selectedItem = (ushort)((selectedItem > 0) ? selectedItem - 1 : LIST_CAPACITY - 1);
        UpdateUI();
        EquipItem();
    }
}