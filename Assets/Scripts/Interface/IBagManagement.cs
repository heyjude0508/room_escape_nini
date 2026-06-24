using System.Collections.Generic;
using UnityEngine;

public interface IBagManagement
{
    // 把物品放进包里
    void AddItem(Item item);

    // 把物品从包里移除
    void RemoveItem(Item item);

    public List<string> GetItemIdList();

    // 检查包里有没有某件物品
    bool HasItem(Item item);

}