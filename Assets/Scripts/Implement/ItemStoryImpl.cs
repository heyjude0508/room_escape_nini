using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStoryImpl : MonoBehaviour, IItemStory
{
    const string DefaultStoryText = "default text";
    const string StoryPanelName = "StoryPanel";
    const string StoryTextName = "StoryText";
    const string CloseButtonName = "CloseButton";

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
