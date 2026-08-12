using UnityEngine;

[System.Serializable]
public class ItemKey : ItemBase
{
    public ItemKey(): base()
    {
        id = "Default Key";
        itemName = "Default Key";
        itemSprite = null;
        itemActionDesc = "Press E to pick up the key.";
        itemUsageDesc = "Default Usage";
    }

}
