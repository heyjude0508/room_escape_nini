using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class LockPuzzle: Puzzle
{
    public LockPuzzle()
    {
        id = "DefaultLock";
        puzzleName = "DefaultLock";
        puzzleDesc = "It is locked.";                                  
        solutionId = "Default Key";
        solveDesc = "Press E to unlock.";
        isSolved = false;

        animationSource = null;
        solveAnimationName = null;

        audioSource = null;
        unsolvedSound = null;
        solvedSound = null;

        puzzleCollider = null;
}

}
