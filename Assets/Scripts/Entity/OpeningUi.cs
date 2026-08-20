using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class OpeningUi
{
    public string backgroundName;
    public string titleTextName;
    public string startGameButtonName;
    public string startGameLabelName;
    public string copyrightTextName;

    public Image backgroundImage;
    public TMP_Text titleText;
    public RectTransform startGameHitArea;
    public TMP_Text startGameLabel;
    public TMP_Text copyrightText;

    public string nextSceneName;
    public float hoverScale;
    public float hoverAnimSpeed;
    public Color normalLabelColor;
    public Color hoverLabelColor;

    public OpeningUi(
        string backgroundName,
        string titleTextName,
        string startGameButtonName,
        string startGameLabelName,
        string copyrightTextName,
        Image backgroundImage,
        TMP_Text titleText,
        RectTransform startGameHitArea,
        TMP_Text startGameLabel,
        TMP_Text copyrightText,
        string nextSceneName,
        float hoverScale,
        float hoverAnimSpeed,
        Color normalLabelColor,
        Color hoverLabelColor)
    {
        this.backgroundName = backgroundName;
        this.titleTextName = titleTextName;
        this.startGameButtonName = startGameButtonName;
        this.startGameLabelName = startGameLabelName;
        this.copyrightTextName = copyrightTextName;
        this.backgroundImage = backgroundImage;
        this.titleText = titleText;
        this.startGameHitArea = startGameHitArea;
        this.startGameLabel = startGameLabel;
        this.copyrightText = copyrightText;
        this.nextSceneName = nextSceneName;
        this.hoverScale = hoverScale;
        this.hoverAnimSpeed = hoverAnimSpeed;
        this.normalLabelColor = normalLabelColor;
        this.hoverLabelColor = hoverLabelColor;
    }

    public OpeningUi()
    {
        backgroundName = "Background";
        titleTextName = "TitleText";
        startGameButtonName = "StartGameButton";
        startGameLabelName = "StartGameLabel";
        copyrightTextName = "CopyrightText";

        nextSceneName = "HouseChild";
        hoverScale = 1.08f;
        hoverAnimSpeed = 12f;
        normalLabelColor = new Color(1f, 0.894f, 0.71f, 1f);
        hoverLabelColor = new Color(1f, 0.78f, 0.35f, 1f);
    }
}
