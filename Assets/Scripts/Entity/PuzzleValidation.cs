using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class PuzzleValidation : PuzzleBase
{
    // Comb lock: plain digits (e.g. 0724).
    // Rolling cart bag: Dad: (id, id), (id, id), (id, id) | Mom: (id, id), (id, id), (id, id)
    // Slot order is free; each (...) must contribute exactly one of its ids, and that id
    // appears only once among the three submitted items.
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
