public interface IWebsiteUi
{
    void AutoFindReferences();

    void Show();

    void Close();

    void SelectTab(int index);

    bool IsWebsiteOpen();
}
