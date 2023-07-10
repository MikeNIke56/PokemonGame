using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory { Items, Tms };

public class Inventory : MonoBehaviour, ISavable
{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> tmSlots;

    List<List<ItemSlot>> allSlots;

    public event Action OnUpdated;

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { slots, tmSlots };

        if(MenuSelection.isHard == true)
        {
            Debug.Log("Hard Enabled");
            foreach(var item in slots)
            {
                if(item.Item.Name != "Mega Crystal" || item.Item.Name != "Mega Crystal X" || item.Item.Name != "Mega Crystal Y" || item.Item.Name != "Blue Orb" || item.Item.Name != "Red Orb")
                {
                    DecreaseItem(item.Item, item.Count / 2);
                }          
            }
        }
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "Items", "TMs"
    };

    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }

    public ItemBase GetItem(int itemIndex, int selectedCat)
    {
        var currentSlots = GetSlotsByCategory(selectedCat);
        return currentSlots[itemIndex].Item;
    }

    public ItemBase UseItem(int indexItem, PokemonInfo selectedPokemon, int selectedCategory)
    {
        var item = GetItem(indexItem, selectedCategory);
        return UseItem(item, selectedPokemon);
    }

    public ItemBase UseItem(ItemBase item, PokemonInfo selectedPokemon)
    {
        bool itemUsed = item.Use(selectedPokemon);

        if (itemUsed)
        {
            DecreaseItem(item);
            return item;
        }
        return null;
    }


    public void DecreaseItem(ItemBase item, int countToRemove=1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count -= countToRemove;
        if (itemSlot.Count == 0)
            currentSlots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }

    ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItems || item is EvoItem)
            return ItemCategory.Items;
        else
            return ItemCategory.Tms;
    }

    public static Inventory GetInventory()
    {
       return FindObjectOfType<PlayerMovement>().GetComponent<Inventory>();
    }

    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
            items = slots.Select(i => i.GetSaveData()).ToList(),
            tms = tmSlots.Select(i => i.GetSaveData()).ToList(),
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;

        slots = saveData.items.Select(i => new ItemSlot(i)).ToList();
        tmSlots = saveData.tms.Select(i => new ItemSlot(i)).ToList();

        allSlots = new List<List<ItemSlot>>() { slots, tmSlots };

        OnUpdated?.Invoke();
    }
}


[Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemSlot()
    {

    }

    public ItemSlot(ItemSaveData saveData)
    {
        item = ItemDB.GetObjectByName(saveData.name);
        count = saveData.count;
    }

    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData()
        {
            name = item.name,
            count = count
        };

        return saveData;
    }

    public ItemBase Item
    {
        get => item;
        set => item = value;
    }
    public int Count
    {
        get => count;
        set => count = value;
    }
}

[Serializable]
public class ItemSaveData
{
    public string name;
    public int count;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items;
    public List<ItemSaveData> tms;
}
