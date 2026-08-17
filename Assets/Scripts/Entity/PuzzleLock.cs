using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class PuzzleLock: PuzzleBase
{
    public string keyId;
    public string unlockDesc;
    public bool bypassKeyRequirement;

    public PuzzleLock()
    {
        id = "DefaultLock";
        puzzleName = "DefaultLock";
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
