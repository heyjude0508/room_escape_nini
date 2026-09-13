using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PrologueStory
{
    public string storyPanelName;
    public string storyImageName;
    public string storyTextName;
    public string prevKeyName;
    public string nextKeyName;
    public string closeKeyName;
    public string storyResourcePath;
    public string storyDesc;

    public Transform storyPanel;
    public Image storyImage;
    public TMP_Text storyText;
    public Button prevButton;
    public Button nextButton;
    public Button closeButton;

    public float hoverScale;
    public float hoverAnimSpeed;

    public int cnt;

    public PrologueStory()
    {
        storyPanelName = "StoryPanel";
        storyImageName = "StoryImage";
        storyTextName = "StoryText";
        prevKeyName = "PrevKey";
        nextKeyName = "NextKey";
        closeKeyName = "CloseKey";
        storyResourcePath = "Materials/PrologueStory";
        storyDesc = "Press E to look at the drawings";

        storyPanel = null;
        storyImage = null;
        storyText = null;
        prevButton = null;
        nextButton = null;
        closeButton = null;

        hoverScale = 1.08f;
        hoverAnimSpeed = 12f;

        cnt = -1;
    }
}
