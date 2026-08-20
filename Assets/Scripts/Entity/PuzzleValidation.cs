using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class PuzzleValidation : PuzzleBase
{
    public string validationCode;
    public string validationDesc;

    public PuzzleValidation() : base()
    {
        id = "Default Validation";
        puzzleName = "Default Validation";
        isSolved = false;

        validationCode = "Default Code";
        validationDesc = "Press E to input the code.";

        animationSource = null;
        solveAnimationName = null;

        audioSource = null;
        unsolvedSound = null;
        solvedSound = null;

        puzzleCollider = null;
    }

}
