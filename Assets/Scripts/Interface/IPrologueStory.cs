public interface IPrologueStory : IPlayerBase
{
    void AutoFindReferences();

    void PlayStory();

    void HideStory();

    bool IsStoryOpen();

    void ShowNextPage();

    void ShowPrevPage();
}
