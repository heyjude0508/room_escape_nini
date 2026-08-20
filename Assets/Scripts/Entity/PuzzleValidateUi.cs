using System;
using UnityEngine;

[Serializable]
public class PuzzleValidateUi
{
    public int digitCount;
    public string hintTip;
    public string hintCorrect;
    public string hintError;

    public PuzzleValidateUi(int digitCount, string hintTip, string hintCorrect, string hintError)
    {
        this.digitCount = digitCount;
        this.hintTip = hintTip;
        this.hintCorrect = hintCorrect;
        this.hintError = hintError;
    }

    public PuzzleValidateUi()
    {
        digitCount = 5;
    }
}
