using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[System.Serializable]
public class LockPuzzle: Puzzle
{
    public LockPuzzle() : base()
    {
        id = "DefaultLock";
        puzzleName = "DefaultLock";
        puzzleDesc = "Press E to unlock.";
        solution = "DefaultKey";
    }

}
