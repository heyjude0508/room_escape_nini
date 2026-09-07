public interface IPrologueUi
{
    void AutoFindReferences();

    void PlayLines(string line, string avatar);

    void PlayTip();

    void HideTip();

    void PulseEnterKey();

    void ResetEnterKeyScale();

    bool IsEnterPressed();

    void HideLines();
}
