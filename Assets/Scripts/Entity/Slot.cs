using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Slot
{
    public string itemId;
    public string itemName;
    public Image iconImage;

    public Slot(string itemId, string itemName, Image iconImage)
    {
        this.itemId = itemId;
        this.itemName = itemName;
        this.iconImage = iconImage;
    }
}