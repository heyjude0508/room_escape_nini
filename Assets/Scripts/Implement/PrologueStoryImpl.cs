using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrologueStoryImpl : MonoBehaviour, IPrologueStory
{
    PrologueStory prologueStory;

    FirstPersonController firstPersonController;
    PrologueSequencer prologueSequencer;
    Dictionary<int, Sprite> storySprites = new Dictionary<int, Sprite>();
    bool buttonsBound;

    Vector3 prevButtonBaseScale = Vector3.one;
    Vector3 nextButtonBaseScale = Vector3.one;
    Vector3 closeButtonBaseScale = Vector3.one;
    bool hasCachedButtonScales;

    void Awake()
    {
        if (prologueStory == null)
        {
            prologueStory = new PrologueStory();
        }

        AutoFindReferences();
        CacheButtonBaseScales();
        SetStoryPanelActive(false);
    }

    void Update()
    {
        if (!IsStoryOpen())
        {
            return;
        }

        UpdateButtonHoverVisual(prologueStory.prevButton, prevButtonBaseScale);
        UpdateButtonHoverVisual(prologueStory.nextButton, nextButtonBaseScale);
        UpdateButtonHoverVisual(prologueStory.closeButton, closeButtonBaseScale);
    }

    public void AutoFindReferences()
    {
        if (prologueStory == null)
        {
            prologueStory = new PrologueStory();
        }

        if (firstPersonController == null)
        {
            firstPersonController = FindObjectOfType<FirstPersonController>();
        }

        if (prologueSequencer == null)
        {
            prologueSequencer = FindObjectOfType<PrologueSequencer>();
        }

        if (string.IsNullOrEmpty(prologueStory.storyPanelName))
        {
            prologueStory.storyPanelName = "StoryPanel";
        }

        if (string.IsNullOrEmpty(prologueStory.storyImageName))
        {
            prologueStory.storyImageName = "StoryImage";
        }

        if (string.IsNullOrEmpty(prologueStory.storyTextName))
        {
            prologueStory.storyTextName = "StoryText";
        }

        if (string.IsNullOrEmpty(prologueStory.prevKeyName))
        {
            prologueStory.prevKeyName = "PrevKey";
        }

        if (string.IsNullOrEmpty(prologueStory.nextKeyName))
        {
            prologueStory.nextKeyName = "NextKey";
        }

        if (string.IsNullOrEmpty(prologueStory.closeKeyName))
        {
            prologueStory.closeKeyName = "CloseKey";
        }

        if (string.IsNullOrEmpty(prologueStory.storyResourcePath))
        {
            prologueStory.storyResourcePath = "Materials/PrologueStory";
        }

        if (string.IsNullOrEmpty(prologueStory.storyDesc))
        {
            prologueStory.storyDesc = "Press E to look at the drawings";
        }

        if (prologueStory.storyPanel == null)
        {
            prologueStory.storyPanel = FindStoryPanel();
        }

        if (prologueStory.storyPanel != null)
        {
            if (prologueStory.storyImage == null)
            {
                Transform imageTransform = FindChildRecursive(prologueStory.storyPanel, prologueStory.storyImageName);
                if (imageTransform != null)
                {
                    prologueStory.storyImage = imageTransform.GetComponent<Image>();
                }
            }

            if (prologueStory.storyText == null)
            {
                Transform textTransform = FindChildRecursive(prologueStory.storyPanel, prologueStory.storyTextName);
                if (textTransform != null)
                {
                    prologueStory.storyText = textTransform.GetComponent<TMP_Text>();
                }
            }

            if (prologueStory.prevButton == null)
            {
                Transform prevTransform = FindChildRecursive(prologueStory.storyPanel, prologueStory.prevKeyName);
                if (prevTransform != null)
                {
                    prologueStory.prevButton = prevTransform.GetComponent<Button>();
                }
            }

            if (prologueStory.nextButton == null)
            {
                Transform nextTransform = FindChildRecursive(prologueStory.storyPanel, prologueStory.nextKeyName);
                if (nextTransform != null)
                {
                    prologueStory.nextButton = nextTransform.GetComponent<Button>();
                }
            }

            if (prologueStory.closeButton == null)
            {
                Transform closeTransform = FindChildRecursive(prologueStory.storyPanel, prologueStory.closeKeyName);
                if (closeTransform != null)
                {
                    prologueStory.closeButton = closeTransform.GetComponent<Button>();
                }
            }
        }

        LoadStorySprites();
        BindButtons();

        if (prologueStory.storyPanel == null)
        {
            Debug.LogError("StoryPanel not found in scene.");
        }

        if (prologueStory.storyImage == null)
        {
            Debug.LogError("StoryImage not found under StoryPanel.");
        }

        if (prologueStory.prevButton == null)
        {
            Debug.LogError("PrevKey Button not found under StoryPanel.");
        }

        if (prologueStory.nextButton == null)
        {
            Debug.LogError("NextKey Button not found under StoryPanel.");
        }

        if (prologueStory.closeButton == null)
        {
            Debug.LogError("CloseKey Button not found under StoryPanel.");
        }

        if (firstPersonController == null)
        {
            Debug.LogError("FirstPersonController not found in scene.");
        }

        CacheButtonBaseScales();
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
        if (prologueStory == null)
        {
            return string.Empty;
        }

        if (IsStoryOpen())
        {
            return string.Empty;
        }

        return prologueStory.storyDesc ?? string.Empty;
    }

    public void Interact()
    {
        if (IsStoryOpen())
        {
            return;
        }

        PlayStory();
    }

    public void PlayStory()
    {
        if (prologueStory == null)
        {
            prologueStory = new PrologueStory();
        }

        if (prologueStory.storyPanel == null || prologueStory.storyImage == null)
        {
            AutoFindReferences();
        }

        if (prologueStory.storyPanel == null)
        {
            return;
        }

        prologueStory.cnt = -1;
        prologueStory.cnt++;
        RefreshStoryPage();

        SetPlayerLocked(true);
        SetStoryPanelActive(true);
    }

    public void HideStory()
    {
        if (prologueStory == null)
        {
            prologueStory = new PrologueStory();
        }

        ResetButtonScales();
        prologueStory.cnt = -1;
        SetStoryPanelActive(false);
        SetPlayerLocked(false);
    }

    public bool IsStoryOpen()
    {
        if (prologueStory == null || prologueStory.storyPanel == null)
        {
            return false;
        }

        return prologueStory.storyPanel.gameObject.activeSelf;
    }

    public void ShowNextPage()
    {
        if (prologueStory == null)
        {
            return;
        }

        int nextCnt = prologueStory.cnt + 1;
        if (!HasStorySprite(nextCnt))
        {
            return;
        }

        prologueStory.cnt = nextCnt;
        RefreshStoryPage();
    }

    public void ShowPrevPage()
    {
        if (prologueStory == null)
        {
            return;
        }

        if (prologueStory.cnt <= 0)
        {
            return;
        }

        prologueStory.cnt--;
        RefreshStoryPage();
    }

    void RefreshStoryPage()
    {
        if (prologueStory == null)
        {
            return;
        }

        if (prologueStory.storyImage != null)
        {
            Sprite sprite;
            if (storySprites != null
                && storySprites.TryGetValue(prologueStory.cnt, out sprite)
                && sprite != null)
            {
                prologueStory.storyImage.sprite = sprite;
                prologueStory.storyImage.enabled = true;
            }
            else
            {
                Debug.LogError("Story sprite not found for cnt=" + prologueStory.cnt);
            }
        }

        RefreshStoryText();
        UpdatePrevButtonVisibility();
    }

    void RefreshStoryText()
    {
        if (prologueStory == null || prologueStory.storyText == null)
        {
            return;
        }

        if (prologueSequencer == null)
        {
            prologueSequencer = FindObjectOfType<PrologueSequencer>();
        }

        if (prologueSequencer == null || prologueSequencer.StoryTextList == null)
        {
            return;
        }

        IReadOnlyList<string> texts = prologueSequencer.StoryTextList;
        if (prologueStory.cnt < 0 || prologueStory.cnt >= texts.Count)
        {
            Debug.LogError("Story text not found for cnt=" + prologueStory.cnt);
            return;
        }

        prologueStory.storyText.text = texts[prologueStory.cnt] ?? string.Empty;
    }

    void UpdatePrevButtonVisibility()
    {
        if (prologueStory == null || prologueStory.prevButton == null)
        {
            return;
        }

        bool showPrev = prologueStory.cnt > 0;
        prologueStory.prevButton.gameObject.SetActive(showPrev);
        if (showPrev)
        {
            prologueStory.prevButton.transform.localScale = prevButtonBaseScale;
        }
    }

    void CacheButtonBaseScales()
    {
        if (prologueStory == null)
        {
            return;
        }

        if (prologueStory.prevButton != null)
        {
            prevButtonBaseScale = prologueStory.prevButton.transform.localScale;
        }

        if (prologueStory.nextButton != null)
        {
            nextButtonBaseScale = prologueStory.nextButton.transform.localScale;
        }

        if (prologueStory.closeButton != null)
        {
            closeButtonBaseScale = prologueStory.closeButton.transform.localScale;
        }

        hasCachedButtonScales = prologueStory.prevButton != null
            || prologueStory.nextButton != null
            || prologueStory.closeButton != null;
    }

    void UpdateButtonHoverVisual(Button button, Vector3 baseScale)
    {
        if (button == null || !button.gameObject.activeInHierarchy)
        {
            return;
        }

        if (!hasCachedButtonScales)
        {
            CacheButtonBaseScales();
        }

        RectTransform hitArea = button.transform as RectTransform;
        if (hitArea == null)
        {
            return;
        }

        bool isHovered = RectTransformUtility.RectangleContainsScreenPoint(
            hitArea,
            Input.mousePosition,
            null);

        float hoverScale = prologueStory.hoverScale > 0f ? prologueStory.hoverScale : 1.08f;
        float animSpeed = prologueStory.hoverAnimSpeed > 0f ? prologueStory.hoverAnimSpeed : 12f;
        float targetScaleFactor = isHovered ? hoverScale : 1f;
        Vector3 targetScale = baseScale * targetScaleFactor;
        float t = 1f - Mathf.Exp(-animSpeed * Time.unscaledDeltaTime);
        hitArea.localScale = Vector3.Lerp(hitArea.localScale, targetScale, t);
    }

    void ResetButtonScales()
    {
        if (prologueStory == null)
        {
            return;
        }

        if (prologueStory.prevButton != null)
        {
            prologueStory.prevButton.transform.localScale = prevButtonBaseScale;
        }

        if (prologueStory.nextButton != null)
        {
            prologueStory.nextButton.transform.localScale = nextButtonBaseScale;
        }

        if (prologueStory.closeButton != null)
        {
            prologueStory.closeButton.transform.localScale = closeButtonBaseScale;
        }
    }

    void LoadStorySprites()
    {
        if (storySprites == null)
        {
            storySprites = new Dictionary<int, Sprite>();
        }
        else
        {
            storySprites.Clear();
        }

        Sprite[] sprites = Resources.LoadAll<Sprite>(prologueStory.storyResourcePath);
        for (int i = 0; i < sprites.Length; i++)
        {
            Sprite sprite = sprites[i];
            if (sprite == null || string.IsNullOrEmpty(sprite.name))
            {
                continue;
            }

            string[] parts = sprite.name.Split('_');
            if (parts.Length < 1)
            {
                continue;
            }

            int index;
            if (!int.TryParse(parts[0], out index))
            {
                continue;
            }

            storySprites[index] = sprite;
        }
    }

    bool HasStorySprite(int index)
    {
        return storySprites != null && storySprites.ContainsKey(index);
    }

    void BindButtons()
    {
        if (buttonsBound)
        {
            return;
        }

        if (prologueStory.prevButton != null)
        {
            prologueStory.prevButton.onClick.RemoveListener(ShowPrevPage);
            prologueStory.prevButton.onClick.AddListener(ShowPrevPage);
        }

        if (prologueStory.nextButton != null)
        {
            prologueStory.nextButton.onClick.RemoveListener(ShowNextPage);
            prologueStory.nextButton.onClick.AddListener(ShowNextPage);
        }

        if (prologueStory.closeButton != null)
        {
            prologueStory.closeButton.onClick.RemoveListener(HideStory);
            prologueStory.closeButton.onClick.AddListener(HideStory);
        }

        buttonsBound = prologueStory.prevButton != null
            || prologueStory.nextButton != null
            || prologueStory.closeButton != null;
    }

    void SetStoryPanelActive(bool active)
    {
        if (prologueStory == null)
        {
            return;
        }

        if (prologueStory.storyPanel == null)
        {
            prologueStory.storyPanel = FindStoryPanel();
        }

        if (prologueStory.storyPanel != null)
        {
            prologueStory.storyPanel.gameObject.SetActive(active);
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

        firstPersonController.playerCanMove = !locked;
        firstPersonController.cameraCanMove = !locked;

        if (locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Rigidbody rigidbody = firstPersonController.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                Vector3 velocity = rigidbody.velocity;
                velocity.x = 0f;
                velocity.z = 0f;
                rigidbody.velocity = velocity;
            }

            AudioSource walkAudioSource = firstPersonController.GetComponent<AudioSource>();
            if (walkAudioSource != null && walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    Transform FindStoryPanel()
    {
        if (prologueStory == null || string.IsNullOrEmpty(prologueStory.storyPanelName))
        {
            return null;
        }

        Transform local = FindChildRecursive(transform, prologueStory.storyPanelName);
        if (local != null)
        {
            return local;
        }

        Canvas[] canvases = FindObjectsOfType<Canvas>(true);
        for (int i = 0; i < canvases.Length; i++)
        {
            if (canvases[i] == null)
            {
                continue;
            }

            Transform found = FindChildRecursive(canvases[i].transform, prologueStory.storyPanelName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
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
