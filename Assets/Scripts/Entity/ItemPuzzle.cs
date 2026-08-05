using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class ItemPuzzle: Puzzle
{
    public string itemId;
    public string placeDesc;
    public GameObject item;

    public ItemPuzzle()
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
