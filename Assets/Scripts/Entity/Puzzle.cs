using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Puzzle
{
    public string id;
    public string puzzleName;
    public string puzzleDesc;
    public string solutionId;
    public string solveDesc;
    public bool isSolved;
    
    public Animation animationSource;
    public string solveAnimationName;

    public AudioSource audioSource;
    public AudioClip unsolvedSound;
    public AudioClip solvedSound;

    public Collider puzzleCollider;

    public Puzzle(
    string id,
    string puzzleName,
    string puzzleDesc,
    string solutionId,
    bool isSolved,
    Animation animationSource,
    string solveAnimationName,
    AudioSource audioSource,
    AudioClip unsolvedSound,
    AudioClip solvedSound,
    Collider puzzleCollider
    )
    {
        this.id = id;
        this.puzzleName = puzzleName;
        this.puzzleDesc = puzzleDesc;
        this.solutionId = solutionId;
        this.isSolved = isSolved;
        this.solveAnimationName = solveAnimationName;
        this.animationSource = animationSource;
        this.audioSource = audioSource;
        this.unsolvedSound = unsolvedSound;
        this.solvedSound = solvedSound;
        this.puzzleCollider = puzzleCollider;
    }

    public Puzzle() { }

}
