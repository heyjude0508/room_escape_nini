using UnityEngine;

public interface IPuzzleValidateUi
{
    void AutoFindReferences();

    void BindButtons();

    void Show();

    void Close();

    void ResetDigits();

    void ChangeDigit(int index, int delta);

    string GetEnteredCode();

    void ShowCorrectThenClose();

    void ShowErrorThenReset();
}
