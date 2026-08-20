using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PuzzleBase
{
    public string id;
    public string puzzleName;
    public bool isSolved;
    
    public Animation animationSource;
    public string solveAnimationName;

    public AudioSource audioSource;
    public AudioClip unsolvedSound;
    public AudioClip solvedSound;

    public Collider puzzleCollider;

    public PuzzleBase(
    string id,
    string puzzleName,
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
        this.isSolved = isSolved;
        this.solveAnimationName = solveAnimationName;
        this.animationSource = animationSource;
        this.audioSource = audioSource;
        this.unsolvedSound = unsolvedSound;
        this.solvedSound = solvedSound;
        this.puzzleCollider = puzzleCollider;
    }

    public PuzzleBase() { }

}
