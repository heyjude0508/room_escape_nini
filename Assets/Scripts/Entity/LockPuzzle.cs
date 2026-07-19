using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class LockPuzzle: Puzzle
{
    public string keyId;
    public string unlockDesc;

    public LockPuzzle()
    {
        id = "DefaultLock";
        puzzleName = "DefaultLock";
        puzzleDesc = "It is locked.";                                  
        isSolved = false;

        keyId = "Default Key";
        unlockDesc = "Press E to unlock.";

        animationSource = null;
        solveAnimationName = null;

        audioSource = null;
        unsolvedSound = null;
        solvedSound = null;

        puzzleCollider = null;
}

}
