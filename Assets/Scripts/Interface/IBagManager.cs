using System.Collections.Generic;
using UnityEngine;

public interface IBagManager
{
    void AddItem(ItemBase item);

    void RemoveItem(ItemBase item);

    void RemoveItem(string itemId);

    List<string> GetItemIdList();

    public bool HasItem(ItemBase item);

    bool HasItem(string itemId);

}
