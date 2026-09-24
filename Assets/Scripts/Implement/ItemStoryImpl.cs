using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStoryImpl : MonoBehaviour, IItemStory
{
    const string DefaultStoryText = "default text";
    const string StoryPanelName = "StoryPanel";
    const string PaperName = "Paper";
    const string StoryTextName = "StoryText";
    const string CloseButtonName = "CloseButton";
    const string PaperResourcePath = "UI/Story/StoryPaper";
    const float PaperScreenRatio = 0.9f;

    static int openStoryCount;

    [SerializeField] ItemStory itemStory;

    Transform storyPanel;
    TMP_Text storyText;
    Button closeButton;
    CanvasGroup storyCanvasGroup;
    FirstPersonController firstPersonController;
    bool closeButtonBound;
    bool playerCanMoveBeforeOpen = true;
    bool cameraCanMoveBeforeOpen = true;

    void Awake()
    {
        if (itemStory == null)
        {
            itemStory = new ItemStory();
        }

        EnsureDefaults();
        AutoFindReferences();
        BindCloseButton();
        SetStoryPanelVisible(false);
    }

    void OnDestroy()
    {
        if (IsStoryOpen())
        {
            openStoryCount = Mathf.Max(0, openStoryCount - 1);
            SetCursorForUi(false);
            SetPlayerLocked(false);
        }
    }

    public void AutoFindReferences()
    {
        if (itemStory == null)
        {
            itemStory = new ItemStory();
        }

        EnsureDefaults();

        if (firstPersonController == null)
        {
            firstPersonController = FindObjectOfType<FirstPersonController>();
        }

        if (storyPanel == null)
        {
            storyPanel = FindStoryPanel();
        }

        if (storyPanel == null)
        {
            Debug.LogError("StoryPanel not found under Canvas in scene.", this);
            return;
        }

        EnsureOpaqueFullscreenBackground();
        EnsurePaperBackground();

        storyCanvasGroup = storyPanel.GetComponent<CanvasGroup>();
        if (storyCanvasGroup == null)
        {
            storyCanvasGroup = storyPanel.GetComponentInChildren<CanvasGroup>(true);
        }

        if (storyText == null)
        {
            Transform textTransform = FindChildRecursive(storyPanel, StoryTextName);
            if (textTransform != null)
            {
                storyText = textTransform.GetComponent<TMP_Text>();
            }
        }

        if (closeButton == null)
        {
            closeButton = FindButton(storyPanel, CloseButtonName);
        }

        ParentStoryContentToPaper();

        if (storyText == null)
        {
            Debug.LogError("StoryText not found under StoryPanel.", this);
        }

        if (closeButton == null)
        {
            Debug.LogError("CloseButton not found under StoryPanel.", this);
        }
    }

    public void EventAimStart()
    {
    }

    public void EventAimEnd()
    {
    }

    public void EventInteract()
    {
    }

    public string GetDescription()
    {
        if (itemStory == null)
        {
            return string.Empty;
        }

        if (IsStoryOpen())
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(itemStory.itemActionDesc))
        {
            return "Press E to read";
        }

        return itemStory.itemActionDesc;
    }

    public void Interact()
    {
        if (IsStoryOpen())
        {
            return;
        }

        ReadStory();
    }

    public void ReadStory()
    {
        if (itemStory == null)
        {
            itemStory = new ItemStory();
        }

        EnsureDefaults();
        AutoFindReferences();
        BindCloseButton();

        if (storyPanel == null)
        {
            return;
        }

        string displayText = string.IsNullOrWhiteSpace(itemStory.storyContent)
            ? DefaultStoryText
            : itemStory.storyContent;

        if (storyText != null)
        {
            storyText.text = displayText;
        }

        SetPlayerLocked(true);
        SetStoryPanelVisible(true);
        SetCursorForUi(true);
    }

    public void HideStory()
    {
        SetStoryPanelVisible(false);
        SetCursorForUi(false);
        SetPlayerLocked(false);
    }

    public bool IsStoryOpen()
    {
        if (storyPanel == null)
        {
            return false;
        }

        if (storyCanvasGroup != null)
        {
            return storyCanvasGroup.alpha > 0.01f && storyCanvasGroup.interactable;
        }

        return storyPanel.gameObject.activeSelf;
    }

    public static bool IsAnyStoryOpen()
    {
        return openStoryCount > 0;
    }

    void EnsureDefaults()
    {
        if (string.IsNullOrWhiteSpace(itemStory.storyContent))
        {
            itemStory.storyContent = DefaultStoryText;
        }

        if (string.IsNullOrWhiteSpace(itemStory.itemActionDesc))
        {
            itemStory.itemActionDesc = "Press E to read";
        }

        if (string.IsNullOrWhiteSpace(itemStory.id))
        {
            itemStory.id = "Default Story";
        }

        if (string.IsNullOrWhiteSpace(itemStory.itemName))
        {
            itemStory.itemName = "Default Story";
        }
    }

    void BindCloseButton()
    {
        if (closeButtonBound || closeButton == null)
        {
            return;
        }

        closeButton.onClick.RemoveListener(HideStory);
        closeButton.onClick.AddListener(HideStory);
        closeButtonBound = true;
    }

    void EnsureOpaqueFullscreenBackground()
    {
        RectTransform panelRect = storyPanel as RectTransform;
        if (panelRect != null)
        {
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
        }

        // Built-in UISprite/Knob have soft alpha edges; null sprite draws a solid quad.
        Image background = storyPanel.GetComponent<Image>();
        if (background == null)
        {
            background = storyPanel.gameObject.AddComponent<Image>();
        }

        background.sprite = null;
        background.type = Image.Type.Simple;
        background.color = Color.black;
        background.raycastTarget = true;
    }

    void EnsurePaperBackground()
    {
        Transform paper = FindChildRecursive(storyPanel, PaperName);
        if (paper == null)
        {
            GameObject paperObject = new GameObject(PaperName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            paper = paperObject.transform;
            paper.SetParent(storyPanel, false);
            paperObject.layer = storyPanel.gameObject.layer;
        }

        RectTransform paperRect = paper as RectTransform;
        if (paperRect != null)
        {
            float margin = (1f - PaperScreenRatio) * 0.5f;
            paperRect.anchorMin = new Vector2(margin, margin);
            paperRect.anchorMax = new Vector2(1f - margin, 1f - margin);
            paperRect.pivot = new Vector2(0.5f, 0.5f);
            paperRect.anchoredPosition = Vector2.zero;
            paperRect.sizeDelta = Vector2.zero;
            paperRect.offsetMin = Vector2.zero;
            paperRect.offsetMax = Vector2.zero;
            paperRect.SetAsFirstSibling();
        }

        Image paperImage = paper.GetComponent<Image>();
        if (paperImage == null)
        {
            paperImage = paper.gameObject.AddComponent<Image>();
        }

        Sprite paperSprite = Resources.Load<Sprite>(PaperResourcePath);
        if (paperSprite != null)
        {
            paperImage.sprite = paperSprite;
        }

        paperImage.type = Image.Type.Simple;
        paperImage.preserveAspect = true;
        paperImage.color = Color.white;
        paperImage.raycastTarget = true;
    }

    void ParentStoryContentToPaper()
    {
        Transform paper = FindChildRecursive(storyPanel, PaperName);
        if (paper == null)
        {
            return;
        }

        if (storyText != null && storyText.transform.parent != paper)
        {
            storyText.transform.SetParent(paper, false);
        }
    }

    void SetStoryPanelVisible(bool visible)
    {
        if (storyPanel == null)
        {
            return;
        }

        bool wasOpen = IsStoryOpen();

        if (storyCanvasGroup != null)
        {
            storyCanvasGroup.alpha = visible ? 1f : 0f;
            storyCanvasGroup.interactable = visible;
            storyCanvasGroup.blocksRaycasts = visible;
            if (!storyPanel.gameObject.activeSelf)
            {
                storyPanel.gameObject.SetActive(true);
            }
        }
        else
        {
            storyPanel.gameObject.SetActive(visible);
        }

        bool isOpen = IsStoryOpen();
        if (!wasOpen && isOpen)
        {
            openStoryCount++;
        }
        else if (wasOpen && !isOpen)
        {
            openStoryCount = Mathf.Max(0, openStoryCount - 1);
        }
    }

    void SetPlayerLocked(bool locked)
    {
        if (firstPersonController == null)
        {
            firstPersonController = FindObjectOfType<FirstPersonController>();
        }

        if (firstPersonController == null)
        {
            return;
        }

        if (locked)
        {
            playerCanMoveBeforeOpen = firstPersonController.playerCanMove;
            cameraCanMoveBeforeOpen = firstPersonController.cameraCanMove;
            firstPersonController.playerCanMove = false;
            firstPersonController.cameraCanMove = false;

            Rigidbody rigidbody = firstPersonController.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                Vector3 velocity = rigidbody.velocity;
                velocity.x = 0f;
                velocity.z = 0f;
                rigidbody.velocity = velocity;
            }

            return;
        }

        firstPersonController.playerCanMove = playerCanMoveBeforeOpen;
        firstPersonController.cameraCanMove = cameraCanMoveBeforeOpen;
    }

    static void SetCursorForUi(bool uiOpen)
    {
        Cursor.visible = uiOpen;
        Cursor.lockState = uiOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    Transform FindStoryPanel()
    {
        // Prefer screen Canvas panels used by HouseChild (BagPanel / ActPanel style).
        Canvas[] canvases = FindObjectsOfType<Canvas>(true);
        for (int i = 0; i < canvases.Length; i++)
        {
            Canvas canvas = canvases[i];
            if (canvas == null || canvas.renderMode == RenderMode.WorldSpace)
            {
                continue;
            }

            Transform found = FindChildRecursive(canvas.transform, StoryPanelName);
            if (found != null)
            {
                return found;
            }
        }

        for (int i = 0; i < canvases.Length; i++)
        {
            if (canvases[i] == null)
            {
                continue;
            }

            Transform found = FindChildRecursive(canvases[i].transform, StoryPanelName);
            if (found != null)
            {
                return found;
            }
        }

        return FindChildRecursive(transform, StoryPanelName);
    }

    static Button FindButton(Transform root, string buttonName)
    {
        Transform found = FindChildRecursive(root, buttonName);
        return found != null ? found.GetComponent<Button>() : null;
    }

    static Transform FindChildRecursive(Transform parent, string childName)
    {
        if (parent == null || string.IsNullOrEmpty(childName))
        {
            return null;
        }

        if (parent.name == childName)
        {
            return parent;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform found = FindChildRecursive(parent.GetChild(i), childName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}
