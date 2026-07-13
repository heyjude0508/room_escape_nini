using System.Collections.Generic;
using UnityEngine;

public interface IBagManagement
{
    void AddItem(Item item);

    void RemoveItem(Item item);

    void RemoveItem(string itemId);

    List<string> GetItemIdList();

    bool HasItem(Item item);

    bool HasItem(string itemId);

}
