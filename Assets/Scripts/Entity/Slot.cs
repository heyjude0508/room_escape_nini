using System;
using UnityEngine;

[Serializable]
public class Slot
{
    public string itemId;
    public string displayName;
    public Sprite icon;

    public bool IsEmpty => string.IsNullOrEmpty(itemId);

    public void Clear()
    {
        itemId = string.Empty;
        displayName = string.Empty;
        icon = null;
    }
}
