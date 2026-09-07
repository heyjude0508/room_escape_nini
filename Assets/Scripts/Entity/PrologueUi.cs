using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PrologueUi
{
    public string subtitlePanelName;
    public string tipPanelName;
    public string linesName;
    public string avatarName;
    public string enterKeyName;
    public TMP_Text linesText;
    public Image avatarImage;
    public RectTransform enterKey;
    public Vector3 enterKeyBaseScale;

    public PrologueUi()
    {
        subtitlePanelName = "SubtitlePanel";
        tipPanelName = "TipPanel";
        linesName = "Lines";
        avatarName = "Avatar";
        enterKeyName = "EnterKey";
        linesText = null;
        avatarImage = null;
        enterKey = null;
        enterKeyBaseScale = Vector3.one;
    }
}
