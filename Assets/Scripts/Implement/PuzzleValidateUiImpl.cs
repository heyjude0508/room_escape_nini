using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleValidateUiImpl : MonoBehaviour, IPuzzleValidateUi
{
    const string PanelName = "Panel";
    const string DigitRowName = "DigitRow";
    const string CloseButtonName = "CloseButton";
    const string ResetButtonName = "ResetButton";
    const string UnlockButtonName = "UnlockButton";
    const string DigitColumnPrefix = "Digit_";
    const string UpButtonName = "UpButton";
    const string DownButtonName = "DownButton";
    const string DigitValueName = "DigitValue";
    const string DigitTextName = "DigitText";

    [SerializeField] PuzzleValidateUi puzzleValidateUi;

    Button closeButton;
    Button resetButton;
    Button unlockButton;
    Transform digitRow;
    Button[] upButtons;
    Button[] downButtons;
    TMP_Text[] digitTexts;
    int[] currentDigits;
    bool buttonsBound;

    void Awake()
    {
        if (puzzleValidateUi == null)
        {
            puzzleValidateUi = new PuzzleValidateUi();
        }

        AutoFindReferences();
        BindButtons();
        ResetDigits();
    }

    public void AutoFindReferences()
    {
        if (puzzleValidateUi == null)
        {
            puzzleValidateUi = new PuzzleValidateUi();
        }

        if (puzzleValidateUi.digitCount <= 0)
        {
            Debug.LogError("PuzzleValidateUi.digitCount must be > 0 on " + name);
            return;
        }

        Transform panelTransform = FindChildRecursive(transform, PanelName);
        Transform searchRoot = panelTransform != null ? panelTransform : transform;

        digitRow = FindChildRecursive(searchRoot, DigitRowName);
        closeButton = FindButton(searchRoot, CloseButtonName);
        resetButton = FindButton(searchRoot, ResetButtonName);
        unlockButton = FindButton(searchRoot, UnlockButtonName);

        EnsureDigitArrays();

        for (int i = 0; i < puzzleValidateUi.digitCount; i++)
        {
            Transform column = digitRow != null
                ? digitRow.Find(DigitColumnPrefix + i)
                : FindChildRecursive(searchRoot, DigitColumnPrefix + i);

            if (column == null)
            {
                continue;
            }

            upButtons[i] = FindButton(column, UpButtonName);
            downButtons[i] = FindButton(column, DownButtonName);

            Transform value = column.Find(DigitValueName);
            Transform textTransform = value != null
                ? value.Find(DigitTextName)
                : FindChildRecursive(column, DigitTextName);
            if (textTransform != null)
            {
                digitTexts[i] = textTransform.GetComponent<TMP_Text>();
            }
        }
    }

    public void BindButtons()
    {
        if (puzzleValidateUi == null || buttonsBound)
        {
            return;
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseClicked);
            closeButton.onClick.AddListener(OnCloseClicked);
            ApplyHoverDarkStyle(closeButton);
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(OnResetClicked);
            resetButton.onClick.AddListener(OnResetClicked);
            ApplyHoverDarkStyle(resetButton);
        }

        if (unlockButton != null)
        {
            unlockButton.onClick.RemoveListener(OnUnlockClicked);
            unlockButton.onClick.AddListener(OnUnlockClicked);
        }

        EnsureDigitArrays();
        for (int i = 0; i < puzzleValidateUi.digitCount; i++)
        {
            int index = i;
            if (upButtons[i] != null)
            {
                upButtons[i].onClick.RemoveAllListeners();
                upButtons[i].onClick.AddListener(() => ChangeDigit(index, 1));
            }

            if (downButtons[i] != null)
            {
                downButtons[i].onClick.RemoveAllListeners();
                downButtons[i].onClick.AddListener(() => ChangeDigit(index, -1));
            }
        }

        buttonsBound = true;
    }

    public void Show()
    {
        if (puzzleValidateUi == null)
        {
            puzzleValidateUi = new PuzzleValidateUi();
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        AutoFindReferences();
        BindButtons();
        ResetDigits();
        SetCursorForUi(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        SetCursorForUi(false);
    }

    public void ResetDigits()
    {
        if (puzzleValidateUi == null)
        {
            return;
        }

        EnsureDigitArrays();
        for (int i = 0; i < puzzleValidateUi.digitCount; i++)
        {
            currentDigits[i] = 0;
        }

        RefreshDigitTexts();
    }

    public void ChangeDigit(int index, int delta)
    {
        if (puzzleValidateUi == null)
        {
            return;
        }

        EnsureDigitArrays();
        if (index < 0 || index >= puzzleValidateUi.digitCount)
        {
            return;
        }

        int value = currentDigits[index] + delta;
        value %= 10;
        if (value < 0)
        {
            value += 10;
        }

        currentDigits[index] = value;
        RefreshDigitTexts();
    }

    public string GetEnteredCode()
    {
        if (puzzleValidateUi == null || currentDigits == null)
        {
            return string.Empty;
        }

        EnsureDigitArrays();
        char[] chars = new char[puzzleValidateUi.digitCount];
        for (int i = 0; i < puzzleValidateUi.digitCount; i++)
        {
            chars[i] = (char)('0' + currentDigits[i]);
        }

        return new string(chars);
    }

    void OnCloseClicked()
    {
        Close();
    }

    void OnResetClicked()
    {
        ResetDigits();
    }

    void OnUnlockClicked()
    {
        PuzzleValidationImpl validation = GetComponentInParent<PuzzleValidationImpl>();
        if (validation == null)
        {
            Debug.LogError("PuzzleValidationImpl not found for Unlock on " + name);
            return;
        }

        validation.ValidateCode(GetEnteredCode());
    }

    static void ApplyHoverDarkStyle(Button button)
    {
        if (button == null)
        {
            return;
        }

        Image image = button.targetGraphic as Image;
        if (image == null)
        {
            image = button.GetComponent<Image>();
        }

        if (image != null)
        {
            if (image.color.a < 0.01f || image.color.maxColorComponent < 0.05f)
            {
                image.color = new Color(0.25f, 0.23f, 0.2f, 1f);
            }

            image.raycastTarget = true;
        }

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.08f, 0.08f, 0.08f, 1f);
        colors.pressedColor = new Color(0.02f, 0.02f, 0.02f, 1f);
        colors.selectedColor = new Color(0.08f, 0.08f, 0.08f, 1f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        button.transition = Selectable.Transition.ColorTint;
        if (image != null)
        {
            button.targetGraphic = image;
        }
    }

    void EnsureDigitArrays()
    {
        int count = puzzleValidateUi.digitCount;
        if (count <= 0)
        {
            Debug.LogError("PuzzleValidateUi.digitCount must be > 0 on " + name);
            return;
        }

        if (upButtons == null || upButtons.Length != count)
        {
            upButtons = new Button[count];
        }

        if (downButtons == null || downButtons.Length != count)
        {
            downButtons = new Button[count];
        }

        if (digitTexts == null || digitTexts.Length != count)
        {
            digitTexts = new TMP_Text[count];
        }

        if (currentDigits == null || currentDigits.Length != count)
        {
            currentDigits = new int[count];
        }
    }

    void RefreshDigitTexts()
    {
        if (digitTexts == null || currentDigits == null)
        {
            return;
        }

        for (int i = 0; i < puzzleValidateUi.digitCount; i++)
        {
            if (digitTexts[i] != null)
            {
                digitTexts[i].text = currentDigits[i].ToString();
            }
        }
    }

    static Button FindButton(Transform root, string name)
    {
        Transform found = FindChildRecursive(root, name);
        return found != null ? found.GetComponent<Button>() : null;
    }

    static void SetCursorForUi(bool uiOpen)
    {
        Cursor.visible = uiOpen;
        Cursor.lockState = uiOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    static Transform FindChildRecursive(Transform parent, string name)
    {
        if (parent == null)
        {
            return null;
        }

        if (parent.name == name)
        {
            return parent;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform found = FindChildRecursive(parent.GetChild(i), name);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}
