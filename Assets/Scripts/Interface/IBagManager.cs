using System.Collections.Generic;
using UnityEngine;

public interface IBagManager
{
    bool AddItem(ItemBase item);

    bool AddFragment(ItemBase item);

    void RemoveItem(string itemId);

    bool HasItem(string itemId);

    List<string> GetItemIdList();

}
