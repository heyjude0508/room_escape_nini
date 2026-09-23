using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStory
{
    public string id;
    public string itemName;
    public string storyContent;

    public ItemStory(
        string id,
        string itemName,
        string storyContent)
    {
        this.id = id;
        this.itemName = itemName;
        this.storyContent = storyContent;
    }

    public ItemStory() { }
}
