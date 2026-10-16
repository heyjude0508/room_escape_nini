using UnityEngine;

[System.Serializable]
public class ItemTrigger : ItemBase
{
    public ItemTrigger(): base()
    {
        id = "Default Trigger";
        itemName = "Default Trigger";
        itemSprite = null;
        itemActionDesc = "Default Desc";
        itemUsageDesc = "Default Usage";
    }

}
