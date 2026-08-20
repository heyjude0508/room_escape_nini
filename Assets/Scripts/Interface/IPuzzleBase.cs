using UnityEngine;

public interface IPuzzleBase : IPlayerBase
{
    void PlaySolveAnimation();

    void PlaySound(AudioClip sound);

    Collider FindSolidCollider();

}
