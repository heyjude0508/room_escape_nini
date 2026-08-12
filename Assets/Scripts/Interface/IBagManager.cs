using System.Collections.Generic;
using UnityEngine;

public interface IBagManager
{
    void AddItem(ItemBase item);

    void RemoveItem(string itemId);

    bool HasItem(string itemId);

    List<string> GetItemIdList();

}
