using UnityEngine;

public interface IBasePuzzle : IBaseAction
{
    bool CanSolve();

    void PlaySolveAnimation();

    void PlaySound(AudioClip sound);

    Collider FindSolidCollider();

}
