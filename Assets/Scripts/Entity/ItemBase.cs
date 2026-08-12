using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemBase
{
    public string id;
    public string itemName;
    public Sprite itemSprite;
    public string itemActionDesc;
    public string itemUsageDesc = "Default Usage";

    public ItemBase(
        string id,
        string itemName,
        string itemActionDesc,
        Sprite itemSprite,
        string itemUsageDesc = "Default Usage")
    {
        this.id = id;
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.itemActionDesc = itemActionDesc;
        this.itemUsageDesc = itemUsageDesc;
    }

    public ItemBase() { }

}
