using UnityEngine;

public interface IPuzzleBase : IPlayerBase
{
    bool CanSolve();

    void PlaySolveAnimation();

    void PlaySound(AudioClip sound);

    Collider FindSolidCollider();

}
