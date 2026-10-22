using UnityEngine;
using UnityEngine.UI;

public class WebsiteUiImpl : MonoBehaviour, IWebsiteUi
{
    const string KitchenUrl = "https://www.johnny4games.com/kitchen/";
    const string GooglePageName = "GooglePage";
    const string KitchenPageName = "KitchenPage";
    const string SnakePageName = "SnakePage";
    const string TabGoogleName = "TabGoogle";
    const string TabKitchenName = "TabKitchen";
    const string TabSnakeName = "TabSnake";

    [SerializeField] GameObject googlePage;
    [SerializeField] GameObject kitchenPage;
    [SerializeField] GameObject snakePage;
    [SerializeField] Image tabGoogleBg;
    [SerializeField] Image tabKitchenBg;
    [SerializeField] Image tabSnakeBg;
    [SerializeField] Color activeTabColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] Color inactiveTabColor = new Color(0.86f, 0.88f, 0.91f, 1f);

    FirstPersonController firstPersonController;
    bool playerCanMoveBeforeOpen = true;
    bool cameraCanMoveBeforeOpen = true;
    bool isShown;
    static int openCount;

    public static bool IsOpen => openCount > 0;

    void Update()
    {
        if (!isShown)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    void OnDisable()
    {
        if (!isShown)
        {
            return;
        }

        isShown = false;
        openCount = Mathf.Max(0, openCount - 1);
        SetCursorForUi(false);
        SetPlayerLocked(false);
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
    }

    public void Show()
    {
        if (isShown)
        {
            return;
        }

        AutoFindReferences();
        isShown = true;
        openCount++;
        gameObject.SetActive(true);
        SetPlayerLocked(true);
        SetCursorForUi(true);
        SelectTab(0);
    }

    public void Close()
    {
        if (!isShown)
        {
            return;
        }

        isShown = false;
        openCount = Mathf.Max(0, openCount - 1);
        SetCursorForUi(false);
        SetPlayerLocked(false);
        gameObject.SetActive(false);
    }

    public void SelectTab(int index)
    {
        AutoFindReferences();

        if (googlePage) googlePage.SetActive(index == 0);
        if (kitchenPage) kitchenPage.SetActive(index == 1);
        if (snakePage) snakePage.SetActive(index == 2);
        if (tabGoogleBg) tabGoogleBg.color = index == 0 ? activeTabColor : inactiveTabColor;
        if (tabKitchenBg) tabKitchenBg.color = index == 1 ? activeTabColor : inactiveTabColor;
        if (tabSnakeBg) tabSnakeBg.color = index == 2 ? activeTabColor : inactiveTabColor;

        if (index == 1)
        {
            Application.OpenURL(KitchenUrl);
        }
    }

    public bool IsWebsiteOpen()
    {
        return isShown;
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
