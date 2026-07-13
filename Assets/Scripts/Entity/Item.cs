using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string id;
    public string itemName;
    public Sprite itemSprite;
    public string itemDesc;

    public Item(string id, string itemName, string itemDesc, Sprite itemSprite)
    {
        this.id = id;
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.itemDesc = itemDesc;
    }

    public Item() { }

}
