using UnityEngine;

public interface IEnvProp : IPlayerBase
{
    void ChangeStatus();

    void PlayAnimation(string animationName);

    void PlaySound(AudioClip sound);
}
