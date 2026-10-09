using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BagManagementImpl : MonoBehaviour, IBagManager
{
    public const int MaxItemSlots = 8;
    public const int MaxFragmentSlots = 6;

    public List<ItemBase> itemList = new List<ItemBase>();
    public List<ItemBase> fragmentList = new List<ItemBase>();
    public List<string> itemIdList = new List<string>();

    public static BagManagementImpl Instance { get; private set; }

    public event Action OnBagUpdated;

    public static bool IsFragmentId(string itemId)
    {
        return !string.IsNullOrEmpty(itemId)
            && itemId.StartsWith("F", StringComparison.Ordinal);
    }

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

    public void AddItem(ItemBase item)
    {
        if (item == null)
        {
            return;
        }

        if (IsFragmentId(item.id))
        {
            AddFragment(item);
            return;
        }

        if (HasItem(item.id))
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

    public void AddFragment(ItemBase item)
    {
        if (item == null)
        {
            return;
        }

        if (HasItem(item.id))
        {
            return;
        }

        if (fragmentList.Count >= MaxFragmentSlots)
        {
            Debug.LogWarning("The fragment slots are full!");
            return;
        }

        fragmentList.Add(item);
        Debug.Log($"Put fragment {item.id} into the bag successfully, total fragments: {fragmentList.Count}.");

        OnBagUpdated?.Invoke();
    }

    public void RemoveItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return;
        }

        ItemBase existingItem = itemList.FirstOrDefault(existing => existing.id == itemId);
        if (existingItem != null)
        {
            itemList.Remove(existingItem);
            Debug.Log($"Get item {itemId} out of the bag successfully, total number of items: {itemList.Count}.");
            OnBagUpdated?.Invoke();
            return;
        }

        ItemBase existingFragment = fragmentList.FirstOrDefault(existing => existing.id == itemId);
        if (existingFragment == null)
        {
            return;
        }

        fragmentList.Remove(existingFragment);
        Debug.Log($"Get fragment {itemId} out of the bag successfully, total fragments: {fragmentList.Count}.");
        OnBagUpdated?.Invoke();
    }

    public bool HasItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return false;
        }

        return itemList.Any(existingItem => existingItem.id == itemId)
            || fragmentList.Any(existingItem => existingItem.id == itemId);
    }

    public List<string> GetItemIdList()
    {
        itemIdList.Clear();
        foreach (ItemBase item in itemList)
        {
            itemIdList.Add(item.id);
        }

        foreach (ItemBase fragment in fragmentList)
        {
            itemIdList.Add(fragment.id);
        }

        return itemIdList;
    }

}
