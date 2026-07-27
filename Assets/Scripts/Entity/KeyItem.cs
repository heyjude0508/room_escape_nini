using UnityEngine;

[System.Serializable]
public class KeyItem : Item
{
    public KeyItem() : base()
    {
        id = "DefaultKey";
        itemName = "DefaultKey";
        itemSprite = null;
        itemActionDesc = "Press E to pick up the key.";
        itemUsageDesc = "Default Usage";
    }

}
