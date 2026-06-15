using UnityEngine;

public interface IBagManagement
{
    void AddItem(string itemId, Sprite icon = null, string displayName = null);

    void RemoveItem(string itemId);

    bool HasItem(string itemId);
}
