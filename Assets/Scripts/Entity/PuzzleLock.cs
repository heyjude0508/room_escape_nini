using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class PuzzleLock: PuzzleBase
{
    public string puzzleDesc;
    public string keyId;
    public string unlockDesc;
    public bool bypassKeyRequirement;

    public PuzzleLock() : base()
    {
        id = "Default Lock";
        puzzleName = "Default Lock";
        puzzleDesc = "It is locked.";                                  
        isSolved = false;

        keyId = "Default Key";
        unlockDesc = "Press E to unlock.";
        bypassKeyRequirement = false;

        animationSource = null;
        solveAnimationName = null;

        audioSource = null;
        unsolvedSound = null;
        solvedSound = null;

        puzzleCollider = null;
    }

}
