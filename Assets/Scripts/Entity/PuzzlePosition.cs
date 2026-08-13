using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class PuzzlePosition: PuzzleBase
{
    public string socketId;
    public string placeDesc;
    [HideInInspector] public GameObject originalItem;

    public PuzzlePosition()
    {
        id = "DefaultSocket";
        puzzleName = "DefaultSocket";
        puzzleDesc = "Something is missing here.";                                  
        isSolved = false;

        socketId = "Default Socket";
        placeDesc = "Press E to place item.";
        originalItem = null;

        animationSource = null;
        solveAnimationName = null;

        audioSource = null;
        unsolvedSound = null;
        solvedSound = null;

        puzzleCollider = null;
    }

}
