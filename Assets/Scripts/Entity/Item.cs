using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string id;
    public string itemName;
    public string itemDesc;
    public Sprite itemSprite;

    public Item(string id, string itemName, string itemDesc, Sprite itemSprite)
    {
        this.id = id;
        this.itemName = itemName;
        this.itemDesc = itemDesc;
        this.itemSprite = itemSprite;
    }

    public Item() { }

}
