using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Slot
{
    public string itemId;
    public string itemName;
    public Image iconImage;
    public Sprite emptySlotSprite;

    public Slot(string itemId, string itemName, Image iconImage, Sprite emptySlotSprite)
    {
        this.itemId = itemId;
        this.itemName = itemName;
        this.iconImage = iconImage;
        this.emptySlotSprite = emptySlotSprite;
    }

}
