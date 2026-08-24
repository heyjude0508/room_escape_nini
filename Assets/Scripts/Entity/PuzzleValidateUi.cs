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

    public PuzzleValidateUi(int digitCount, float duration, string hintTip, string hintCorrect, string hintError)
    {
        this.digitCount = digitCount;
        this.duration = duration;
        this.hintTip = hintTip;
        this.hintCorrect = hintCorrect;
        this.hintError = hintError;
    }

    public PuzzleValidateUi()
    {
        digitCount = 5;
        duration = 0.5f;
        hintTip = "Click arrows to set the code";
        hintCorrect = "CORRECT CODE";
        hintError = "ERROR! TRY AGAIN";
    }
}
