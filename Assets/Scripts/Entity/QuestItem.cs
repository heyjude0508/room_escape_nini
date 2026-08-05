using UnityEngine;

[System.Serializable]
public class QuestItem : Item
{
    public QuestItem() : base()
    {
        id = "DefaultItem";
        itemName = "DefaultItem";
        itemSprite = null;
        itemActionDesc = "Press E to pick up the item.";
        itemUsageDesc = "Default Usage";
    }

}
