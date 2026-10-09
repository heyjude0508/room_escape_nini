public interface IPlotUi
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
