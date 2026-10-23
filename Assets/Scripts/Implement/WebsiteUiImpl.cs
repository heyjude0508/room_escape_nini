using System;
using Gree.UnityWebView;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WebsiteUiImpl : MonoBehaviour, IWebsiteUi
{
    const string GoogleUrl = "google.com";
    const string KitchenUrl = "https://www.johnny4games.com/kitchen/";
    const string SnakeBaseUrl = "https://www.johnny4games.com/snake/";
    const string GooglePageName = "GooglePage";
    const string KitchenPageName = "KitchenPage";
    const string SnakePageName = "SnakePage";
    const string TabGoogleName = "TabGoogle";
    const string TabKitchenName = "TabKitchen";
    const string TabSnakeName = "TabSnake";
    const string NewTabName = "NewTab";
    const string UrlTextName = "UrlText";
    const string CloseButtonName = "CloseButton";
    const string WebViewObjectName = "WebsiteContentWebView";
    const string KitchenUnlockStoryId = "N005";
    const string SnakeUnlockStoryId = "N011";
    const float TabBarStartX = 8f;
    const float TabSpacing = 8f;
    const float NewTabGap = 12f;

    [SerializeField] GameObject googlePage;
    [SerializeField] GameObject kitchenPage;
    [SerializeField] GameObject snakePage;
    [SerializeField] Image tabGoogleBg;
    [SerializeField] Image tabKitchenBg;
    [SerializeField] Image tabSnakeBg;
    [SerializeField] RectTransform newTabRect;
    [SerializeField] TMP_Text urlText;
    [SerializeField] Button closeButton;
    [SerializeField] Color activeTabColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] Color inactiveTabColor = new Color(0.86f, 0.88f, 0.91f, 1f);

    FirstPersonController firstPersonController;
    bool playerCanMoveBeforeOpen = true;
    bool cameraCanMoveBeforeOpen = true;
    bool isShown;
    bool closeButtonBound;
    static int openCount;
    WebViewObject contentWebView;
    string loadedContentUrl;
    string sessionSid;
    int activeTab = -1;
    bool navigationEventsBeforeSnake = true;
    bool refreshingTabUnlocks;

    public static bool IsOpen => openCount > 0;
    public static string CurrentSessionSid { get; private set; }

    bool IsKitchenUnlocked => ItemStoryImpl.HasReadStory(KitchenUnlockStoryId);
    bool IsSnakeUnlocked => ItemStoryImpl.HasReadStory(SnakeUnlockStoryId);
    bool IsSnakeTabActive => activeTab == 2 && IsSnakeUnlocked;

    string SnakeUrlWithSid =>
        string.IsNullOrEmpty(sessionSid)
            ? SnakeBaseUrl
            : SnakeBaseUrl + "?sid=" + Uri.EscapeDataString(sessionSid);

    void OnEnable()
    {
        ItemStoryImpl.OnStoryRead -= HandleStoryRead;
        ItemStoryImpl.OnStoryRead += HandleStoryRead;
        RefreshTabUnlocks();
    }

    void Update()
    {
        if (!isShown)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
            return;
        }

        ForwardSnakeKeyboardFromUnity();
    }

    void OnDisable()
    {
        ItemStoryImpl.OnStoryRead -= HandleStoryRead;
        SetUiNavigationEventsEnabled(true);
        DestroyContentWebView();

        if (!isShown)
        {
            return;
        }

        isShown = false;
        openCount = Mathf.Max(0, openCount - 1);
        SetCursorForUi(false);
        SetPlayerLocked(false);
    }

    void OnDestroy()
    {
        DestroyContentWebView();
    }

    public void AutoFindReferences()
    {
        if (googlePage == null)
        {
            Transform found = FindChildRecursive(transform, GooglePageName);
            if (found != null)
            {
                googlePage = found.gameObject;
            }
        }

        if (kitchenPage == null)
        {
            Transform found = FindChildRecursive(transform, KitchenPageName);
            if (found != null)
            {
                kitchenPage = found.gameObject;
            }
        }

        if (snakePage == null)
        {
            Transform found = FindChildRecursive(transform, SnakePageName);
            if (found != null)
            {
                snakePage = found.gameObject;
            }
        }

        if (tabGoogleBg == null)
        {
            tabGoogleBg = FindImageOnChild(TabGoogleName);
        }

        if (tabKitchenBg == null)
        {
            tabKitchenBg = FindImageOnChild(TabKitchenName);
        }

        if (tabSnakeBg == null)
        {
            tabSnakeBg = FindImageOnChild(TabSnakeName);
        }

        if (newTabRect == null)
        {
            Transform found = FindChildRecursive(transform, NewTabName);
            if (found != null)
            {
                newTabRect = found as RectTransform;
            }
        }

        if (urlText == null)
        {
            Transform found = FindChildRecursive(transform, UrlTextName);
            if (found != null)
            {
                urlText = found.GetComponent<TMP_Text>();
            }
        }

        if (closeButton == null)
        {
            Transform found = FindChildRecursive(transform, CloseButtonName);
            if (found != null)
            {
                closeButton = found.GetComponent<Button>();
            }
        }

        BindCloseButton();
    }

    void BindCloseButton()
    {
        if (closeButtonBound || closeButton == null)
        {
            return;
        }

        closeButton.onClick.RemoveListener(Close);
        closeButton.onClick.AddListener(Close);
        closeButtonBound = true;
    }

    void HandleStoryRead(string storyId)
    {
        if (storyId == KitchenUnlockStoryId || storyId == SnakeUnlockStoryId)
        {
            RefreshTabUnlocks();
        }
    }

    void RefreshTabUnlocks()
    {
        if (refreshingTabUnlocks)
        {
            return;
        }

        refreshingTabUnlocks = true;
        try
        {
            bool kitchenUnlocked = IsKitchenUnlocked;
            if (tabKitchenBg != null)
            {
                tabKitchenBg.gameObject.SetActive(kitchenUnlocked);
            }

            if (!kitchenUnlocked && kitchenPage != null)
            {
                kitchenPage.SetActive(false);
            }

            bool snakeUnlocked = IsSnakeUnlocked;
            if (tabSnakeBg != null)
            {
                tabSnakeBg.gameObject.SetActive(snakeUnlocked);
            }

            if (!snakeUnlocked && snakePage != null)
            {
                snakePage.SetActive(false);
            }

            RelayoutTabBar();

            if ((!kitchenUnlocked && activeTab == 1) || (!snakeUnlocked && activeTab == 2))
            {
                if (isShown)
                {
                    SelectTab(0);
                }
                else
                {
                    activeTab = 0;
                }
            }
        }
        finally
        {
            refreshingTabUnlocks = false;
        }
    }

    void RelayoutTabBar()
    {
        float x = TabBarStartX;
        bool hasVisibleTab = false;

        PlaceTabIfVisible(tabGoogleBg, ref x, ref hasVisibleTab);
        PlaceTabIfVisible(tabKitchenBg, ref x, ref hasVisibleTab);
        PlaceTabIfVisible(tabSnakeBg, ref x, ref hasVisibleTab);

        if (newTabRect == null)
        {
            return;
        }

        if (hasVisibleTab)
        {
            x = x - TabSpacing + NewTabGap;
        }

        newTabRect.anchoredPosition = new Vector2(x, newTabRect.anchoredPosition.y);
        newTabRect.SetAsLastSibling();
        if (closeButton != null)
        {
            closeButton.transform.SetAsLastSibling();
        }
    }

    static void PlaceTabIfVisible(Image tabBg, ref float x, ref bool hasVisibleTab)
    {
        if (tabBg == null || !tabBg.gameObject.activeSelf)
        {
            return;
        }

        RectTransform tabRect = tabBg.rectTransform;
        tabRect.anchoredPosition = new Vector2(x, tabRect.anchoredPosition.y);
        float width = tabRect.rect.width;
        if (width <= 0.01f)
        {
            width = tabRect.sizeDelta.x;
        }

        x += width + TabSpacing;
        hasVisibleTab = true;
    }

    public void Show()
    {
        if (isShown)
        {
            return;
        }

        BeginNewSession();
        AutoFindReferences();
        isShown = true;
        openCount++;
        gameObject.SetActive(true);
        SetPlayerLocked(true);
        SetCursorForUi(true);
        RefreshTabUnlocks();
        SelectTab(0);
    }

    public void Close()
    {
        if (!isShown)
        {
            return;
        }

        SetUiNavigationEventsEnabled(true);
        DestroyContentWebView();
        isShown = false;
        openCount = Mathf.Max(0, openCount - 1);
        SetCursorForUi(false);
        SetPlayerLocked(false);
        gameObject.SetActive(false);
    }

    public void SelectTab(int index)
    {
        AutoFindReferences();

        if (index == 1 && !IsKitchenUnlocked)
        {
            index = 0;
        }

        if (index == 2 && !IsSnakeUnlocked)
        {
            index = 0;
        }

        activeTab = index;

        if (googlePage) googlePage.SetActive(index == 0);
        if (kitchenPage) kitchenPage.SetActive(index == 1 && IsKitchenUnlocked);
        if (snakePage) snakePage.SetActive(index == 2 && IsSnakeUnlocked);
        if (tabGoogleBg) tabGoogleBg.color = index == 0 ? activeTabColor : inactiveTabColor;
        if (tabKitchenBg) tabKitchenBg.color = index == 1 ? activeTabColor : inactiveTabColor;
        if (tabSnakeBg) tabSnakeBg.color = index == 2 ? activeTabColor : inactiveTabColor;

        if (urlText != null)
        {
            if (index == 0) urlText.text = GoogleUrl;
            else if (index == 1) urlText.text = KitchenUrl;
            else urlText.text = SnakeUrlWithSid;
        }

        if (index == 1 && IsKitchenUnlocked)
        {
            SetUiNavigationEventsEnabled(true);
            ShowContentWebView(KitchenUrl, kitchenPage);
        }
        else if (index == 2 && IsSnakeUnlocked)
        {
            // Prevent EventSystem from eating arrow keys for UI navigation.
            SetUiNavigationEventsEnabled(false);
            ShowContentWebView(SnakeUrlWithSid, snakePage);
            InjectSnakeKeyboardBridge();
        }
        else
        {
            SetUiNavigationEventsEnabled(true);
            HideContentWebView();
        }
    }

    void ForwardSnakeKeyboardFromUnity()
    {
        if (!IsSnakeTabActive || contentWebView == null)
        {
            return;
        }

        // WebView often does not deliver keydown into the page; drive Snake from C#.
        // Prefer __snakeInput (local/redeployed Snake.html), fall back to global setDir.
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            contentWebView.EvaluateJS(
                "if(window.__snakeInput)window.__snakeInput('up');"
                + "else if(typeof setDir==='function')setDir(0,-1);");
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            contentWebView.EvaluateJS(
                "if(window.__snakeInput)window.__snakeInput('down');"
                + "else if(typeof setDir==='function')setDir(0,1);");
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            contentWebView.EvaluateJS(
                "if(window.__snakeInput)window.__snakeInput('left');"
                + "else if(typeof setDir==='function')setDir(-1,0);");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            contentWebView.EvaluateJS(
                "if(window.__snakeInput)window.__snakeInput('right');"
                + "else if(typeof setDir==='function')setDir(1,0);");
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            contentWebView.EvaluateJS(
                "if(window.__snakeInput)window.__snakeInput('pause');"
                + "else if(typeof togglePause==='function')togglePause();"
                + "else if(typeof running!=='undefined'&&running){"
                + "paused=!paused;if(paused){clearInterval(timer);timer=null;}else if(typeof setSpeed==='function')setSpeed();}");
        }
    }

    void SetUiNavigationEventsEnabled(bool enabled)
    {
        if (EventSystem.current == null)
        {
            return;
        }

        if (!enabled)
        {
            navigationEventsBeforeSnake = EventSystem.current.sendNavigationEvents;
            EventSystem.current.sendNavigationEvents = false;
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        EventSystem.current.sendNavigationEvents = navigationEventsBeforeSnake;
    }

    public bool IsWebsiteOpen()
    {
        return isShown;
    }

    void BeginNewSession()
    {
        sessionSid = Guid.NewGuid().ToString("N");
        CurrentSessionSid = sessionSid;
        loadedContentUrl = null;
    }

    void ShowContentWebView(string url, GameObject pageForMargins)
    {
        EnsureContentWebView();
        if (contentWebView == null)
        {
            Debug.LogError("WebViewObject could not be created.");
            return;
        }

        ApplyContentWebViewMargins(pageForMargins);
        contentWebView.SetVisibility(true);
        if (!string.Equals(loadedContentUrl, url, StringComparison.Ordinal))
        {
            contentWebView.LoadURL(url);
            loadedContentUrl = url;
        }
    }

    void HideContentWebView()
    {
        if (contentWebView == null)
        {
            return;
        }

        contentWebView.SetVisibility(false);
    }

    void EnsureContentWebView()
    {
        if (contentWebView != null)
        {
            return;
        }

        GameObject go = new GameObject(WebViewObjectName);
        DontDestroyOnLoad(go);
        contentWebView = go.AddComponent<WebViewObject>();
        contentWebView.Init(
            cb: OnWebViewMessage,
            err: (msg) => Debug.LogError("Website WebView error: " + msg),
            httpErr: (msg) => Debug.LogError("Website WebView http error: " + msg),
            ld: OnContentWebViewLoaded,
            transparent: false,
            zoom: true);
        loadedContentUrl = null;
    }

    void OnContentWebViewLoaded(string url)
    {
        Debug.Log("Website WebView loaded: " + url);
        if (string.IsNullOrEmpty(url) || url.IndexOf("/snake", StringComparison.OrdinalIgnoreCase) < 0)
        {
            return;
        }

        // Ensure keyboard bridge exists even if hosted Snake.html is not redeployed yet.
        InjectSnakeKeyboardBridge();
    }

    void InjectSnakeKeyboardBridge()
    {
        if (contentWebView == null)
        {
            return;
        }

        contentWebView.EvaluateJS(
            "window.__snakeInput=function(cmd){"
            + "if(typeof setDir==='function'){"
            + "if(cmd==='up')setDir(0,-1);"
            + "else if(cmd==='down')setDir(0,1);"
            + "else if(cmd==='left')setDir(-1,0);"
            + "else if(cmd==='right')setDir(1,0);"
            + "}"
            + "if(cmd==='pause'){"
            + "if(typeof togglePause==='function')togglePause();"
            + "else if(typeof running!=='undefined'&&running){"
            + "paused=!paused;if(paused){clearInterval(timer);timer=null;}"
            + "else if(typeof setSpeed==='function')setSpeed();}"
            + "}"
            + "};");
    }

    void DestroyContentWebView()
    {
        if (contentWebView == null)
        {
            return;
        }

        Destroy(contentWebView.gameObject);
        contentWebView = null;
        loadedContentUrl = null;
    }

    void OnWebViewMessage(string message)
    {
        if (string.IsNullOrEmpty(message) || !message.StartsWith("snakeScore:", StringComparison.Ordinal))
        {
            return;
        }

        string[] parts = message.Split(':');
        if (parts.Length < 2)
        {
            return;
        }

        string code = parts[1];
        if (parts.Length >= 3 && !string.IsNullOrEmpty(sessionSid) && parts[2] != sessionSid)
        {
            return;
        }

        ApplyDynamicWebsitePasswords(code);
    }

    void ApplyDynamicWebsitePasswords(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return;
        }

        int applied = 0;
        PuzzleValidationImpl[] puzzles = FindObjectsOfType<PuzzleValidationImpl>(true);
        for (int i = 0; i < puzzles.Length; i++)
        {
            PuzzleValidationImpl puzzle = puzzles[i];
            if (puzzle == null || !puzzle.UsesDynamicWebsitePassword)
            {
                continue;
            }

            puzzle.SetValidationCode(code);
            applied++;
            Debug.Log(
                "Dynamic website password set from Snake score for "
                + puzzle.PuzzleId + " / " + puzzle.gameObject.name + ": " + code);
        }

        if (applied == 0)
        {
            Debug.LogWarning(
                "No puzzle with Validation Code \""
                + PuzzleValidationImpl.DynamicWebsitePasswordCode
                + "\" to apply Snake score password.");
        }
    }

    void ApplyContentWebViewMargins(GameObject pageForMargins)
    {
        if (contentWebView == null)
        {
            return;
        }

        RectTransform pageRect = pageForMargins != null
            ? pageForMargins.GetComponent<RectTransform>()
            : null;

        if (pageRect == null)
        {
            contentWebView.SetMargins(0, 84, 0, 0);
            return;
        }

        Vector3[] corners = new Vector3[4];
        pageRect.GetWorldCorners(corners);
        Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
        Vector2 topRight = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

        int left = Mathf.Max(0, Mathf.RoundToInt(bottomLeft.x));
        int right = Mathf.Max(0, Mathf.RoundToInt(Screen.width - topRight.x));
        int top = Mathf.Max(0, Mathf.RoundToInt(Screen.height - topRight.y));
        int bottom = Mathf.Max(0, Mathf.RoundToInt(bottomLeft.y));
        contentWebView.SetMargins(left, top, right, bottom);
    }

    Image FindImageOnChild(string childName)
    {
        Transform found = FindChildRecursive(transform, childName);
        return found != null ? found.GetComponent<Image>() : null;
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
