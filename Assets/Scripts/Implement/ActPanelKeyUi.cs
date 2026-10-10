using UnityEngine;

public static class ActPanelKeyUi
{
    const string ActPanelName = "ActPanel";
    const string EKeyName = "EKey";
    const string NoEKeyName = "NoEKey";

    static Transform eKey;
    static Transform noEKey;
    static bool resolved;

    public static void SetCanInteract(bool canInteract)
    {
        CacheKeys();
        if (eKey != null)
        {
            eKey.gameObject.SetActive(canInteract);
        }

        if (noEKey != null)
        {
            noEKey.gameObject.SetActive(!canInteract);
        }
    }

    public static void ShowDefaultEKey()
    {
        SetCanInteract(true);
    }

    static void CacheKeys()
    {
        if (resolved && eKey != null && noEKey != null)
        {
            return;
        }

        resolved = true;
        Transform actPanel = FindActPanel();
        if (actPanel == null)
        {
            return;
        }

        eKey = actPanel.Find(EKeyName);
        noEKey = actPanel.Find(NoEKeyName);
    }

    static Transform FindActPanel()
    {
        Canvas[] canvases = Object.FindObjectsOfType<Canvas>(true);
        for (int i = 0; i < canvases.Length; i++)
        {
            Transform found = FindChildRecursive(canvases[i].transform, ActPanelName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    static Transform FindChildRecursive(Transform parent, string childName)
    {
        if (parent == null)
        {
            return null;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == childName)
            {
                return child;
            }

            Transform nested = FindChildRecursive(child, childName);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }
}
