using UnityEngine;

public interface IBagManagement
{
    // 把物品放进包里
    void AddItem(string itemId);

    // 把物品从包里移除
    void RemoveItem(string itemId);

    // 检查包里有没有某件物品
    bool HasItem(string itemId);

}
