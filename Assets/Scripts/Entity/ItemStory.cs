using System;
using UnityEngine;

[Serializable]
public class ItemStory
{
    public string id;
    public string itemName;
    public string storyContent;
    public string itemActionDesc;

    public ItemStory()
    {
        id = "Default Story";
        itemName = "Default Story";
        storyContent = "default text";
        itemActionDesc = "Press E to read";
    }

    public ItemStory(string id, string itemName, string storyContent)
    {
        this.id = id;
        this.itemName = itemName;
        this.storyContent = string.IsNullOrWhiteSpace(storyContent) ? "default text" : storyContent;
        itemActionDesc = "Press E to read";
    }
}
