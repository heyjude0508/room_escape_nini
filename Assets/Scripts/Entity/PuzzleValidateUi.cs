using System;
using UnityEngine;

[Serializable]
public class PuzzleValidateUi
{
    public int digitCount;
    public float duration;
    public string hintTip;
    public string hintCorrect;
    public string hintError;

    [Tooltip("When true, up/down arrows cycle bag items instead of digits 0-9.")]
    public bool selectFromBag;

    [Tooltip("Shown when a bag-select slot has no item selected.")]
    public Sprite emptySlotSprite;

    public PuzzleValidateUi(
        int digitCount,
        float duration,
        string hintTip,
        string hintCorrect,
        string hintError)
    {
        this.digitCount = digitCount;
        this.duration = duration;
        this.hintTip = hintTip;
        this.hintCorrect = hintCorrect;
        this.hintError = hintError;
        selectFromBag = false;
        emptySlotSprite = null;
    }

    public PuzzleValidateUi()
    {
        digitCount = 4;
        duration = 0.5f;
        hintTip = "Click arrows to set the code";
        hintCorrect = "CORRECT CODE";
        hintError = "ERROR! TRY AGAIN";
        selectFromBag = false;
        emptySlotSprite = null;
    }
}
