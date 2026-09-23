using UnityEngine;

public interface IItemStory : IPlayerBase
{
    void AutoFindReferences();

    void ReadStory();

    void HideStory();

    bool IsStoryOpen();
}
