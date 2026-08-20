using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Slot
{
    public string itemId;
    public string itemName;
    public Image frameImage;
    public Image iconImage;
    public Sprite emptySlotSprite;
    public Sprite itemSprite;

    public Slot(
        string itemId,
        string itemName,
        Image frameImage,
        Image iconImage,
        Sprite emptySlotSprite)
    {
        this.itemId = itemId;
        this.itemName = itemName;
        this.frameImage = frameImage;
        this.iconImage = iconImage;
        this.emptySlotSprite = emptySlotSprite;
    }

    public Slot() { }
}
