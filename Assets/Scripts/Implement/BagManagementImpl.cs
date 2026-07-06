using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BagManagementImpl : MonoBehaviour, IBagManagement
{
    public const int MaxItemSlots = 8;

    public List<Item> itemList = new List<Item>();
    public List<string> itemIdList = new List<string>();

    public static BagManagementImpl Instance { get; private set; }

    public event Action OnBagUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(Item item)
    {
        if (item == null)
        {
            return;
        }

        if (itemList.Any(existingItem => existingItem.id == item.id))
        {
            return;
        }

        if (itemList.Count >= MaxItemSlots)
        {
            Debug.LogWarning("The bag is full!");
            return;
        }

        itemList.Add(item);
        Debug.Log($"Put item {item.id} into the bag successfully, total number of items: {itemList.Count}.");

        OnBagUpdated?.Invoke();
    }

    public void RemoveItem(Item item)
    {
        if (item == null || string.IsNullOrEmpty(item.id))
        {
            return;
        }

        RemoveItem(item.id);
    }

    public void RemoveItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return;
        }

        Item existingItem = itemList.FirstOrDefault(existing => existing.id == itemId);
        if (existingItem == null)
        {
            return;
        }

        itemList.Remove(existingItem);
        Debug.Log($"Get item {itemId} out of the bag successfully, total number of items: {itemList.Count}.");

        OnBagUpdated?.Invoke();
    }

    public List<string> GetItemIdList()
    {
        itemIdList.Clear();
        foreach (Item item in itemList)
        {
            itemIdList.Add(item.id);
        }
        return itemIdList;
    }

    public bool HasItem(Item item)
    {
        if (item == null)
        {
            return false;
        }

        return HasItem(item.id);
    }

    public bool HasItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return false;
        }

        return itemList.Any(existingItem => existingItem.id == itemId);
    }
}
