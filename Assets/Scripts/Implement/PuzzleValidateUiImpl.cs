using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleValidateUiImpl : MonoBehaviour, IPuzzleValidateUi
{
    const string PanelName = "DigitInput";
    const string DigitRowName = "DigitRow";
    const string HintName = "Hint";
    const string CloseButtonName = "CloseButton";
    const string ResetButtonName = "ResetButton";
    const string UnlockButtonName = "UnlockButton";
    const string DigitColumnPrefix = "Digit_";
    const string UpButtonName = "UpButton";
    const string DownButtonName = "DownButton";
    const string DigitValueName = "DigitValue";
    const string DigitTextName = "DigitText";
    const string EmptySlotResourcePath = "UI/RollingCartEmptySlot";
    const string BagItemSeparator = ",";
    const string ParentItemSeparator = "|";
    const string EmptyBagHint = "Your bag is empty!";
    const string ParentSubmitSuccessHint = "Submit successfully!";
    const string ParentSelectName = "ParentSelect";
    const string ParentLeftButtonName = "LeftButton";
    const string ParentRightButtonName = "RightButton";
    const string ParentValueName = "ParentValue";
    const string ParentDad = "Dad";
    const string ParentMom = "Mom";
    // LiberationSans has ▲ (same as digit UpButton), not ◀▶ — rotate for left/right.
    const string ParentArrowGlyph = "\u25B2";

    static readonly Color DigitPlateColor = new Color(0.2f, 0.18f, 0.16f, 1f);
    static readonly Color EmptyBagHintColor = new Color(0.92f, 0.18f, 0.16f, 1f);
    static readonly Color DefaultHintColor = new Color(1f, 0.94f, 0.82f, 1f);
    static readonly Color ParentValueColor = new Color(0.72f, 0.58f, 0.38f, 1f);
    static readonly Color ParentArrowColor = new Color(0.25f, 0.23f, 0.2f, 1f);
    static readonly Color ParentSuccessHintColor = new Color(0.35f, 0.85f, 0.4f, 1f);

    [SerializeField] PuzzleValidateUi puzzleValidateUi;

    Button closeButton;
    Button resetButton;
    Button unlockButton;
    Button parentLeftButton;
    Button parentRightButton;
    TMP_Text parentValueText;
    Transform digitRow;
    Transform panelRoot;
    TMP_Text hintText;
    Color hintDefaultColor = DefaultHintColor;
    Button[] upButtons;
    Button[] downButtons;
    TMP_Text[] digitTexts;
    Image[] slotImages;
    Color[] slotImageBaseColors;
    int[] currentDigits;
    readonly List<ItemBase> bagOptions = new List<ItemBase>();
    readonly Dictionary<string, int[]> parentDigitDrafts = new Dictionary<string, int[]>();
    readonly Dictionary<string, string[]> parentLockedItemIds = new Dictionary<string, string[]>();
    readonly Dictionary<string, Sprite[]> parentLockedSprites = new Dictionary<string, Sprite[]>();
    readonly HashSet<string> acceptedParentsLocal = new HashSet<string>();
    string selectedParent = ParentDad;
    bool buttonsBound;
    bool parentButtonsBound;
    bool isFeedbackPlaying;
    Coroutine hintRoutine;
    PuzzleValidationImpl boundValidation;

    public bool SelectsFromBag =>
        puzzleValidateUi != null
        && (puzzleValidateUi.selectFromBag || IsRollingCartPanel());

    bool IsRollingCartPanel()
    {
        return gameObject != null && gameObject.name == "RollingCartPanel";
    }

    void Awake()
    {
        if (puzzleValidateUi == null)
        {
            puzzleValidateUi = new PuzzleValidateUi();
        }

        ApplyRollingCartDefaults();
        EnsureEmptySlotSprite();
        AutoFindReferences();
        BindButtons();
        ResetDigits();
        ShowTipHint();
    }

    void OnEnable()
    {
        if (puzzleValidateUi == null)
        {
            return;
        }

        ApplyRollingCartDefaults();
        EnsureEmptySlotSprite();
        RefreshBagOptions();
        RefreshDigitTexts();
        ShowTipHint();
    }

    void ApplyRollingCartDefaults()
    {
        if (!IsRollingCartPanel() || puzzleValidateUi == null)
        {
            return;
        }

        puzzleValidateUi.selectFromBag = true;
        // Rolling cart uses three bag-item slots.
        puzzleValidateUi.digitCount = 3;

        if (string.IsNullOrWhiteSpace(puzzleValidateUi.hintTip)
            || puzzleValidateUi.hintTip == "Click arrows to set the code")
        {
            puzzleValidateUi.hintTip = "Use arrows to choose an item from your bag";
        }

        if (string.IsNullOrWhiteSpace(puzzleValidateUi.hintCorrect)
            || puzzleValidateUi.hintCorrect == "CORRECT CODE"
            || puzzleValidateUi.hintCorrect == "ITEM ACCEPTED")
        {
            puzzleValidateUi.hintCorrect = ParentSubmitSuccessHint;
        }

        if (string.IsNullOrWhiteSpace(puzzleValidateUi.hintError)
            || puzzleValidateUi.hintError == "ERROR! TRY AGAIN")
        {
            puzzleValidateUi.hintError = "WRONG ITEM";
        }
    }

    public void AutoFindReferences()
    {
        if (puzzleValidateUi == null)
        {
            puzzleValidateUi = new PuzzleValidateUi();
        }

        ApplyRollingCartDefaults();
        EnsureEmptySlotSprite();

        if (puzzleValidateUi.digitCount <= 0)
        {
            Debug.LogError("PuzzleValidateUi.digitCount must be > 0 on " + name);
            return;
        }

        Transform panelTransform = FindChildRecursive(transform, PanelName);
        Transform searchRoot = panelTransform != null ? panelTransform : transform;
        panelRoot = searchRoot;

        digitRow = FindChildRecursive(searchRoot, DigitRowName);
        closeButton = FindButton(searchRoot, CloseButtonName);
        resetButton = FindButton(searchRoot, ResetButtonName);
        unlockButton = FindButton(searchRoot, UnlockButtonName);

        Transform hintTransform = FindChildRecursive(searchRoot, HintName);
        hintText = hintTransform != null ? hintTransform.GetComponent<TMP_Text>() : null;
        if (hintText != null)
        {
            hintDefaultColor = hintText.color;
            if (hintDefaultColor.a < 0.01f)
            {
                hintDefaultColor = DefaultHintColor;
            }
        }

        if (SelectsFromBag || IsRollingCartPanel())
        {
            // Dad/Mom live on DigitInput top-left; switch with left/right arrows.
            EnsureParentSelectUi(searchRoot);
            CacheParentSelectRefs(searchRoot);
            RefreshParentSelectVisuals();
        }

        EnsureDigitArrays();
        SyncDigitColumnVisibility(searchRoot);

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

            // Bag mode draws the item/cat on DigitValue's Image; digit text is hidden.
            if (value != null)
            {
                slotImages[i] = value.GetComponent<Image>();
                if (slotImages[i] != null)
                {
                    slotImageBaseColors[i] = DigitPlateColor;
                }
            }
        }
    }

    void SyncDigitColumnVisibility(Transform searchRoot)
    {
        if (digitRow == null && searchRoot == null)
        {
            return;
        }

        Transform row = digitRow != null ? digitRow : searchRoot;
        for (int i = 0; i < row.childCount; i++)
        {
            Transform child = row.GetChild(i);
            if (child == null || !child.name.StartsWith(DigitColumnPrefix))
            {
                continue;
            }

            string suffix = child.name.Substring(DigitColumnPrefix.Length);
            if (!int.TryParse(suffix, out int index))
            {
                continue;
            }

            child.gameObject.SetActive(index >= 0 && index < puzzleValidateUi.digitCount);
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

        BindParentButtons();

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

    void BindParentButtons()
    {
        if (!SelectsFromBag || parentButtonsBound)
        {
            return;
        }

        if (parentLeftButton != null)
        {
            parentLeftButton.onClick.RemoveAllListeners();
            parentLeftButton.onClick.AddListener(() => CycleParent(-1));
        }

        if (parentRightButton != null)
        {
            parentRightButton.onClick.RemoveAllListeners();
            parentRightButton.onClick.AddListener(() => CycleParent(1));
        }

        parentButtonsBound = true;
    }

    public void ShowFor(PuzzleValidationImpl validation)
    {
        boundValidation = validation;
        Show();
    }

    public void Show()
    {
        if (puzzleValidateUi == null)
        {
            puzzleValidateUi = new PuzzleValidateUi();
        }

        StopHintRoutine();
        isFeedbackPlaying = false;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        ApplyRollingCartDefaults();
        AutoFindReferences();
        // Allow rebinding after digitCount / parent UI may have changed for RollingCart.
        buttonsBound = false;
        parentButtonsBound = false;
        if (SelectsFromBag || IsRollingCartPanel())
        {
            Transform digitInput = panelRoot != null ? panelRoot : FindChildRecursive(transform, PanelName);
            Transform parentRoot = digitInput != null ? digitInput : transform;
            EnsureParentSelectUi(parentRoot);
            CacheParentSelectRefs(parentRoot);
        }

        BindButtons();
        RefreshBagOptions();
        SyncAcceptedParentsFromValidation();
        selectedParent = ParentDad;
        EnsureParentDigitDraft(ParentDad);
        EnsureParentDigitDraft(ParentMom);
        LoadParentDigitDraft(selectedParent);
        RefreshParentSelectVisuals();
        RefreshSlotInteractable();
        RefreshParentContextHint();
        SetCursorForUi(true);
    }

    public void Close()
    {
        StopHintRoutine();
        isFeedbackPlaying = false;
        boundValidation = null;
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
        if (puzzleValidateUi == null || isFeedbackPlaying)
        {
            return;
        }

        EnsureDigitArrays();
        if (index < 0 || index >= puzzleValidateUi.digitCount)
        {
            return;
        }

        if (SelectsFromBag)
        {
            if (IsCurrentParentAccepted())
            {
                return;
            }

            RefreshBagOptions();
            if (bagOptions.Count == 0)
            {
                ShowEmptyBagHint();
                return;
            }

            ShowTipHint();
            int optionCount = GetBagOptionCount();
            int value = currentDigits[index];
            int steps = 0;
            do
            {
                value += delta;
                value %= optionCount;
                if (value < 0)
                {
                    value += optionCount;
                }

                steps++;
                // Empty (0) is always allowed; each bag item may only occupy one slot.
                if (value == 0 || !IsBagOptionTakenByOtherSlot(index, value))
                {
                    break;
                }
            }
            while (steps < optionCount);

            currentDigits[index] = value;
            SaveParentDigitDraft(selectedParent);
            RefreshDigitTexts();
            return;
        }

        int digit = currentDigits[index] + delta;
        digit %= 10;
        if (digit < 0)
        {
            digit += 10;
        }

        currentDigits[index] = digit;
        RefreshDigitTexts();
    }

    public string GetEnteredCode()
    {
        if (puzzleValidateUi == null || currentDigits == null)
        {
            return string.Empty;
        }

        EnsureDigitArrays();

        if (SelectsFromBag)
        {
            RefreshBagOptions();
            string[] ids = new string[puzzleValidateUi.digitCount];
            for (int i = 0; i < puzzleValidateUi.digitCount; i++)
            {
                ids[i] = GetSelectedBagItemId(currentDigits[i]);
            }

            string items = string.Join(BagItemSeparator, ids);
            string parent = string.IsNullOrEmpty(selectedParent) ? ParentDad : selectedParent;
            return parent + ParentItemSeparator + items;
        }

        char[] chars = new char[puzzleValidateUi.digitCount];
        for (int i = 0; i < puzzleValidateUi.digitCount; i++)
        {
            chars[i] = (char)('0' + currentDigits[i]);
        }

        return new string(chars);
    }

    public void ConsumeSelectedBagItems()
    {
        if (!SelectsFromBag)
        {
            return;
        }

        BagManagementImpl bag = BagManagementImpl.Instance;
        if (bag == null)
        {
            return;
        }

        string entered = GetEnteredCode();
        if (string.IsNullOrEmpty(entered))
        {
            return;
        }

        string itemsPart = entered;
        int sep = entered.IndexOf(ParentItemSeparator, System.StringComparison.Ordinal);
        if (sep >= 0 && sep + 1 < entered.Length)
        {
            itemsPart = entered.Substring(sep + 1);
        }

        string[] ids = itemsPart.Split(new[] { BagItemSeparator }, System.StringSplitOptions.None);
        for (int i = 0; i < ids.Length; i++)
        {
            if (!string.IsNullOrEmpty(ids[i]))
            {
                bag.RemoveItem(ids[i]);
            }
        }
    }

    public void ShowCorrectThenClose()
    {
        StopHintRoutine();
        hintRoutine = StartCoroutine(CorrectHintRoutine());
    }

    public void ShowErrorThenReset()
    {
        StopHintRoutine();
        hintRoutine = StartCoroutine(ErrorHintRoutine());
    }

    void OnCloseClicked()
    {
        Close();
    }

    void OnResetClicked()
    {
        if (isFeedbackPlaying)
        {
            return;
        }

        if (SelectsFromBag && IsCurrentParentAccepted())
        {
            return;
        }

        RefreshBagOptions();
        ResetDigits();
        if (SelectsFromBag)
        {
            SaveParentDigitDraft(selectedParent);
        }

        RefreshParentContextHint();
    }

    public void NotifyBagParentAccepted(string parent)
    {
        if (string.IsNullOrEmpty(parent))
        {
            return;
        }

        CaptureLockedSlotsForParent(parent);
        acceptedParentsLocal.Add(parent);
        SaveParentDigitDraft(parent);
        ClearUnacceptedParentDrafts(parent);
        RefreshSlotInteractable();
        RefreshDigitTexts();
    }

    void ClearUnacceptedParentDrafts(string keepParent)
    {
        if (puzzleValidateUi == null)
        {
            return;
        }

        string[] parents = { ParentDad, ParentMom };
        for (int p = 0; p < parents.Length; p++)
        {
            string parent = parents[p];
            if (string.Equals(parent, keepParent, System.StringComparison.Ordinal)
                || IsParentAccepted(parent))
            {
                continue;
            }

            EnsureParentDigitDraft(parent);
            int[] draft = parentDigitDrafts[parent];
            for (int i = 0; i < draft.Length; i++)
            {
                draft[i] = 0;
            }
        }
    }

    public void ShowBagParentAcceptedStayOpen(string parent)
    {
        StopHintRoutine();
        isFeedbackPlaying = false;
        if (!string.IsNullOrEmpty(parent))
        {
            selectedParent = parent;
            LoadParentDigitDraft(selectedParent);
            RefreshParentSelectVisuals();
        }

        RefreshSlotInteractable();
        ShowSubmitSuccessHint();
    }

    public void ShowBagParentAlreadyAccepted(string parent)
    {
        StopHintRoutine();
        isFeedbackPlaying = false;
        if (!string.IsNullOrEmpty(parent))
        {
            selectedParent = parent;
            LoadParentDigitDraft(selectedParent);
            RefreshParentSelectVisuals();
        }

        RefreshSlotInteractable();
        ShowSubmitSuccessHint();
    }

    void CacheParentSelectRefs(Transform searchRoot)
    {
        parentLeftButton = FindButton(searchRoot, ParentLeftButtonName);
        parentRightButton = FindButton(searchRoot, ParentRightButtonName);

        Transform valueTransform = FindChildRecursive(searchRoot, ParentValueName);
        parentValueText = null;
        if (valueTransform != null)
        {
            parentValueText = valueTransform.GetComponent<TMP_Text>();
            if (parentValueText == null)
            {
                Transform label = valueTransform.Find("Label");
                if (label != null)
                {
                    parentValueText = label.GetComponent<TMP_Text>();
                }
            }
        }
    }

    void CycleParent(int direction)
    {
        // Parent switch stays available after a successful Dad/Mom submit.
        if (direction == 0)
        {
            return;
        }

        if (isFeedbackPlaying && !SelectsFromBag)
        {
            return;
        }

        if (SelectsFromBag)
        {
            SaveParentDigitDraft(selectedParent);
        }

        selectedParent = selectedParent == ParentDad ? ParentMom : ParentDad;
        if (SelectsFromBag)
        {
            LoadParentDigitDraft(selectedParent);
            RefreshSlotInteractable();
            RefreshParentContextHint();
        }

        RefreshParentSelectVisuals();
    }

    void RefreshParentSelectVisuals()
    {
        if (parentValueText != null)
        {
            parentValueText.text = string.IsNullOrEmpty(selectedParent) ? ParentDad : selectedParent;
        }
    }

    bool IsCurrentParentAccepted()
    {
        return IsParentAccepted(selectedParent);
    }

    bool IsParentAccepted(string parent)
    {
        if (string.IsNullOrEmpty(parent))
        {
            return false;
        }

        if (acceptedParentsLocal.Contains(parent))
        {
            return true;
        }

        return boundValidation != null && boundValidation.HasAcceptedBagParent(parent);
    }

    void SyncAcceptedParentsFromValidation()
    {
        acceptedParentsLocal.Clear();
        if (boundValidation == null)
        {
            return;
        }

        if (boundValidation.HasAcceptedBagParent(ParentDad))
        {
            acceptedParentsLocal.Add(ParentDad);
        }

        if (boundValidation.HasAcceptedBagParent(ParentMom))
        {
            acceptedParentsLocal.Add(ParentMom);
        }
    }

    void EnsureParentDigitDraft(string parent)
    {
        if (puzzleValidateUi == null || string.IsNullOrEmpty(parent))
        {
            return;
        }

        int count = puzzleValidateUi.digitCount;
        if (!parentDigitDrafts.TryGetValue(parent, out int[] draft) || draft == null || draft.Length != count)
        {
            parentDigitDrafts[parent] = new int[count];
        }
    }

    void SaveParentDigitDraft(string parent)
    {
        if (string.IsNullOrEmpty(parent) || currentDigits == null || puzzleValidateUi == null)
        {
            return;
        }

        EnsureParentDigitDraft(parent);
        int[] draft = parentDigitDrafts[parent];
        int count = Mathf.Min(draft.Length, currentDigits.Length);
        for (int i = 0; i < count; i++)
        {
            draft[i] = currentDigits[i];
        }
    }

    void LoadParentDigitDraft(string parent)
    {
        EnsureDigitArrays();
        EnsureParentDigitDraft(parent);
        int[] draft = parentDigitDrafts[parent];
        int count = Mathf.Min(draft.Length, currentDigits.Length);
        for (int i = 0; i < count; i++)
        {
            currentDigits[i] = draft[i];
        }

        RefreshDigitTexts();
    }

    void CaptureLockedSlotsForParent(string parent)
    {
        if (puzzleValidateUi == null || string.IsNullOrEmpty(parent))
        {
            return;
        }

        EnsureDigitArrays();
        RefreshBagOptions();
        int count = puzzleValidateUi.digitCount;
        string[] ids = new string[count];
        Sprite[] sprites = new Sprite[count];
        for (int i = 0; i < count; i++)
        {
            ids[i] = GetSelectedBagItemId(currentDigits[i]);
            sprites[i] = GetBagOptionSprite(currentDigits[i]);
        }

        parentLockedItemIds[parent] = ids;
        parentLockedSprites[parent] = sprites;
    }

    void RefreshSlotInteractable()
    {
        bool locked = SelectsFromBag && IsCurrentParentAccepted();
        if (upButtons != null)
        {
            for (int i = 0; i < upButtons.Length; i++)
            {
                if (upButtons[i] != null)
                {
                    upButtons[i].interactable = !locked;
                }
            }
        }

        if (downButtons != null)
        {
            for (int i = 0; i < downButtons.Length; i++)
            {
                if (downButtons[i] != null)
                {
                    downButtons[i].interactable = !locked;
                }
            }
        }

        if (resetButton != null && SelectsFromBag)
        {
            resetButton.interactable = !locked;
        }

        if (unlockButton != null && SelectsFromBag)
        {
            unlockButton.interactable = !locked;
        }
    }

    void RefreshParentContextHint()
    {
        if (!SelectsFromBag)
        {
            ShowTipHint();
            return;
        }

        if (IsCurrentParentAccepted())
        {
            ShowSubmitSuccessHint();
            return;
        }

        ShowTipHint();
    }

    void ShowSubmitSuccessHint()
    {
        string text = puzzleValidateUi != null && !string.IsNullOrWhiteSpace(puzzleValidateUi.hintCorrect)
            ? puzzleValidateUi.hintCorrect
            : ParentSubmitSuccessHint;
        SetHintText(text, ParentSuccessHintColor);
    }

    void EnsureParentSelectUi(Transform digitInputRoot)
    {
        if (digitInputRoot == null)
        {
            return;
        }

        // Drop any leftover ParentSelect created under the full-screen panel root.
        Transform stray = transform.Find(ParentSelectName);
        if (stray != null && stray.parent == transform && digitInputRoot != transform)
        {
            Destroy(stray.gameObject);
        }

        Transform existing = FindChildRecursive(digitInputRoot, ParentSelectName);
        if (existing != null)
        {
            // Old Dad/Mom buttons → rebuild as left/right arrows.
            if (FindChildRecursive(existing, ParentLeftButtonName) == null)
            {
                Destroy(existing.gameObject);
                existing = null;
            }
            else
            {
                ApplyParentSelectLayout(existing.GetComponent<RectTransform>(), digitInputRoot);
                existing.gameObject.SetActive(true);
                existing.SetAsLastSibling();
                return;
            }
        }

        GameObject root = new GameObject(ParentSelectName, typeof(RectTransform));
        root.layer = digitInputRoot.gameObject.layer;
        RectTransform rootRect = root.GetComponent<RectTransform>();
        ApplyParentSelectLayout(rootRect, digitInputRoot);

        HorizontalLayoutGroup layout = root.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        TMP_FontAsset font = ResolveParentSelectFont();
        CreateParentArrowButton(rootRect, ParentLeftButtonName, font, 90f);
        CreateParentValuePlate(rootRect, font);
        CreateParentArrowButton(rootRect, ParentRightButtonName, font, -90f);
    }

    static void ApplyParentSelectLayout(RectTransform rootRect, Transform digitInputRoot)
    {
        if (rootRect == null || digitInputRoot == null)
        {
            return;
        }

        rootRect.SetParent(digitInputRoot, false);
        rootRect.anchorMin = new Vector2(0f, 1f);
        rootRect.anchorMax = new Vector2(0f, 1f);
        rootRect.pivot = new Vector2(0f, 1f);
        rootRect.anchoredPosition = new Vector2(18f, -14f);
        rootRect.sizeDelta = new Vector2(220f, 40f);
        rootRect.localScale = Vector3.one;
        rootRect.SetAsLastSibling();
    }

    TMP_FontAsset ResolveParentSelectFont()
    {
        if (hintText != null && hintText.font != null)
        {
            return hintText.font;
        }

        return TMP_Settings.defaultFontAsset;
    }

    static Sprite WhiteUiSprite()
    {
        return Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            100f);
    }

    void CreateParentArrowButton(
        Transform parent,
        string objectName,
        TMP_FontAsset font,
        float labelZRotation)
    {
        GameObject buttonGo = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button),
            typeof(LayoutElement));
        buttonGo.layer = parent.gameObject.layer;
        RectTransform buttonRect = buttonGo.GetComponent<RectTransform>();
        buttonRect.SetParent(parent, false);
        buttonRect.sizeDelta = new Vector2(40f, 40f);

        LayoutElement layoutElement = buttonGo.GetComponent<LayoutElement>();
        layoutElement.preferredWidth = 40f;
        layoutElement.preferredHeight = 40f;
        layoutElement.minWidth = 40f;
        layoutElement.minHeight = 40f;

        Image image = buttonGo.GetComponent<Image>();
        image.sprite = WhiteUiSprite();
        image.type = Image.Type.Simple;
        image.color = ParentArrowColor;
        image.raycastTarget = true;

        Button button = buttonGo.GetComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.35f, 0.32f, 0.28f, 1f);
        colors.pressedColor = new Color(0.18f, 0.16f, 0.14f, 1f);
        colors.selectedColor = Color.white;
        button.colors = colors;

        RectTransform labelRect = CreateParentLabel(buttonRect, "Label", ParentArrowGlyph, font, 22f);
        if (labelRect != null)
        {
            labelRect.localRotation = Quaternion.Euler(0f, 0f, labelZRotation);
        }
    }

    void CreateParentValuePlate(Transform parent, TMP_FontAsset font)
    {
        GameObject valueGo = new GameObject(
            ParentValueName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(LayoutElement));
        valueGo.layer = parent.gameObject.layer;
        RectTransform valueRect = valueGo.GetComponent<RectTransform>();
        valueRect.SetParent(parent, false);
        valueRect.sizeDelta = new Vector2(110f, 40f);

        LayoutElement layoutElement = valueGo.GetComponent<LayoutElement>();
        layoutElement.preferredWidth = 110f;
        layoutElement.preferredHeight = 40f;
        layoutElement.minWidth = 110f;
        layoutElement.minHeight = 40f;

        Image image = valueGo.GetComponent<Image>();
        image.sprite = WhiteUiSprite();
        image.type = Image.Type.Simple;
        image.color = ParentValueColor;
        image.raycastTarget = false;

        CreateParentLabel(valueRect, "Label", ParentDad, font, 22f);
    }

    static RectTransform CreateParentLabel(
        Transform parent,
        string objectName,
        string text,
        TMP_FontAsset font,
        float fontSize)
    {
        GameObject textGo = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        textGo.layer = parent.gameObject.layer;
        RectTransform textRect = textGo.GetComponent<RectTransform>();
        textRect.SetParent(parent, false);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textGo.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = fontSize;
        tmp.color = DefaultHintColor;
        tmp.raycastTarget = false;
        if (font != null)
        {
            tmp.font = font;
        }

        return textRect;
    }

    void OnUnlockClicked()
    {
        if (isFeedbackPlaying)
        {
            return;
        }

        if (SelectsFromBag && IsCurrentParentAccepted())
        {
            ShowSubmitSuccessHint();
            return;
        }

        PuzzleValidationImpl validation = boundValidation;
        if (validation == null)
        {
            validation = GetComponentInParent<PuzzleValidationImpl>();
        }

        if (validation == null)
        {
            Debug.LogError("PuzzleValidationImpl not found for Unlock on " + name);
            return;
        }

        validation.ValidateCode(GetEnteredCode());
    }

    void ShowTipHint()
    {
        SetHintText(puzzleValidateUi != null ? puzzleValidateUi.hintTip : string.Empty, hintDefaultColor);
    }

    void ShowEmptyBagHint()
    {
        SetHintText(EmptyBagHint, EmptyBagHintColor);
    }

    void SetHintText(string text)
    {
        SetHintText(text, hintDefaultColor);
    }

    void SetHintText(string text, Color color)
    {
        if (hintText == null)
        {
            return;
        }

        hintText.text = text ?? string.Empty;
        hintText.color = color;
    }

    IEnumerator CorrectHintRoutine()
    {
        isFeedbackPlaying = true;
        ShowSubmitSuccessHint();
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, puzzleValidateUi.duration));
        isFeedbackPlaying = false;
        hintRoutine = null;
        Close();
    }

    IEnumerator ErrorHintRoutine()
    {
        isFeedbackPlaying = true;
        SetHintText(puzzleValidateUi.hintError);
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, puzzleValidateUi.duration));
        if (!(SelectsFromBag && IsCurrentParentAccepted()))
        {
            ResetDigits();
            if (SelectsFromBag)
            {
                SaveParentDigitDraft(selectedParent);
            }
        }

        RefreshParentContextHint();
        isFeedbackPlaying = false;
        hintRoutine = null;
    }

    void StopHintRoutine()
    {
        if (hintRoutine != null)
        {
            StopCoroutine(hintRoutine);
            hintRoutine = null;
        }
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

        if (slotImages == null || slotImages.Length != count)
        {
            slotImages = new Image[count];
        }

        if (slotImageBaseColors == null || slotImageBaseColors.Length != count)
        {
            slotImageBaseColors = new Color[count];
            for (int i = 0; i < count; i++)
            {
                slotImageBaseColors[i] = DigitPlateColor;
            }
        }

        if (currentDigits == null || currentDigits.Length != count)
        {
            currentDigits = new int[count];
        }
    }

    void RefreshDigitTexts()
    {
        if (currentDigits == null)
        {
            return;
        }

        if (SelectsFromBag)
        {
            RefreshBagSlotVisuals();
            return;
        }

        RestoreDigitModeVisuals();
        if (digitTexts == null)
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

    void RefreshBagSlotVisuals()
    {
        EnsureEmptySlotSprite();
        Sprite[] lockedSprites = null;
        if (IsCurrentParentAccepted())
        {
            parentLockedSprites.TryGetValue(selectedParent, out lockedSprites);
        }

        for (int i = 0; i < puzzleValidateUi.digitCount; i++)
        {
            if (digitTexts[i] != null)
            {
                digitTexts[i].text = string.Empty;
                digitTexts[i].enabled = false;
            }

            if (slotImages[i] == null)
            {
                continue;
            }

            Sprite sprite = null;
            if (lockedSprites != null && i < lockedSprites.Length)
            {
                sprite = lockedSprites[i];
            }

            if (sprite == null)
            {
                sprite = GetBagOptionSprite(currentDigits[i]);
            }

            if (sprite == null)
            {
                EnsureEmptySlotSprite();
                sprite = puzzleValidateUi.emptySlotSprite;
            }

            slotImages[i].sprite = sprite;
            slotImages[i].color = Color.white;
            slotImages[i].preserveAspect = true;
            slotImages[i].type = Image.Type.Simple;
            slotImages[i].enabled = true;
        }
    }

    void RestoreDigitModeVisuals()
    {
        for (int i = 0; i < puzzleValidateUi.digitCount; i++)
        {
            if (digitTexts[i] != null)
            {
                digitTexts[i].enabled = true;
            }

            if (slotImages[i] == null)
            {
                continue;
            }

            slotImages[i].sprite = null;
            slotImages[i].preserveAspect = false;
            slotImages[i].color = slotImageBaseColors[i].a > 0.01f
                ? slotImageBaseColors[i]
                : DigitPlateColor;
        }
    }

    void RefreshBagOptions()
    {
        bagOptions.Clear();
        BagManagementImpl bag = BagManagementImpl.Instance;
        if (bag == null || bag.itemList == null)
        {
            return;
        }

        for (int i = 0; i < bag.itemList.Count; i++)
        {
            ItemBase item = bag.itemList[i];
            if (item != null && !string.IsNullOrEmpty(item.id))
            {
                bagOptions.Add(item);
            }
        }
    }

    int GetBagOptionCount()
    {
        // Index 0 = empty (black cat), then each bag item.
        return Mathf.Max(1, bagOptions.Count + 1);
    }

    bool IsBagOptionTakenByOtherSlot(int slotIndex, int optionIndex)
    {
        string itemId = GetSelectedBagItemId(optionIndex);
        if (string.IsNullOrEmpty(itemId) || currentDigits == null)
        {
            return false;
        }

        for (int i = 0; i < puzzleValidateUi.digitCount; i++)
        {
            if (i == slotIndex)
            {
                continue;
            }

            if (string.Equals(GetSelectedBagItemId(currentDigits[i]), itemId, System.StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    string GetSelectedBagItemId(int optionIndex)
    {
        if (optionIndex <= 0 || optionIndex > bagOptions.Count)
        {
            return string.Empty;
        }

        ItemBase item = bagOptions[optionIndex - 1];
        return item != null ? item.id : string.Empty;
    }

    Sprite GetBagOptionSprite(int optionIndex)
    {
        if (optionIndex <= 0 || optionIndex > bagOptions.Count)
        {
            return puzzleValidateUi != null ? puzzleValidateUi.emptySlotSprite : null;
        }

        ItemBase item = bagOptions[optionIndex - 1];
        if (item != null && item.itemSprite != null)
        {
            return item.itemSprite;
        }

        return puzzleValidateUi != null ? puzzleValidateUi.emptySlotSprite : null;
    }

    void EnsureEmptySlotSprite()
    {
        if (puzzleValidateUi == null)
        {
            return;
        }

        if (puzzleValidateUi.emptySlotSprite != null)
        {
            return;
        }

        puzzleValidateUi.emptySlotSprite = Resources.Load<Sprite>(EmptySlotResourcePath);
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
