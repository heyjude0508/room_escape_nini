using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[System.Serializable]
public class Puzzle
{
    public string id;
    public string puzzleName;
    public string puzzleDesc;
    public string solution;

    public Puzzle(string id, string puzzleName, string puzzleDesc, string solution) 
    {
        this.id = id;
        this.puzzleName = puzzleName;
        this.puzzleDesc = puzzleDesc;
        this.solution = solution;
    }

    public Puzzle() { }



}
