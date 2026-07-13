using UnityEngine;

public interface IPuzzleAction : IPlayerAction
{
    bool CanSolve();

    void PlaySolveAnimation();

    void PlaySound(AudioClip sound);

    Collider FindSolidCollider();

}
