using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class PuzzlePosition: PuzzleBase
{
    public string itemId;
    public string placeDesc;
    public GameObject item;

    public PuzzlePosition()
    {
        id = "DefaultSocket";
        puzzleName = "DefaultSocket";
        puzzleDesc = "Something is missing here.";                                  
        isSolved = false;

        itemId = "Default Item";
        placeDesc = "Press E to place item.";
        item = null;

        animationSource = null;
        solveAnimationName = null;

        audioSource = null;
        unsolvedSound = null;
        solvedSound = null;

        puzzleCollider = null;
    }

}
