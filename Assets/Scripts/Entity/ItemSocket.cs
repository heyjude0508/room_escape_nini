using UnityEngine;

[System.Serializable]
public class ItemSocket : ItemBase
{
    public ItemSocket(): base()
    {
        id = "Default Item";
        itemName = "Default Item";
        itemSprite = null;
        itemActionDesc = "Press E to pick up the item.";
        itemUsageDesc = "Default Usage";
    }

}
