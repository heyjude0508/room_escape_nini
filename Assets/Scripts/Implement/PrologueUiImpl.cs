using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrologueUiImpl : MonoBehaviour, IPrologueUi
{
    const string AvatarResourcePath = "UI/Avatar/";
    const float EnterKeyPulseScale = 1.2f;

    PrologueUi prologueUi;

    void Awake()
    {
        if (prologueUi == null)
        {
            prologueUi = new PrologueUi();
        }

        AutoFindReferences();
        SetSubtitlePanelActive(false);
    }

    public void AutoFindReferences()
    {
        if (prologueUi == null)
        {
            prologueUi = new PrologueUi();
        }

        if (string.IsNullOrEmpty(prologueUi.linesName))
        {
            prologueUi.linesName = "Lines";
        }

        if (string.IsNullOrEmpty(prologueUi.subtitlePanelName))
        {
            prologueUi.subtitlePanelName = "SubtitlePanel";
        }

        if (string.IsNullOrEmpty(prologueUi.avatarName))
        {
            prologueUi.avatarName = "Avatar";
        }

        if (string.IsNullOrEmpty(prologueUi.enterKeyName))
        {
            prologueUi.enterKeyName = "EnterKey";
        }

        if (prologueUi.linesText == null)
        {
            Transform linesTransform = FindChildRecursive(transform, prologueUi.linesName);
            if (linesTransform != null)
            {
                prologueUi.linesText = linesTransform.GetComponent<TMP_Text>();
            }
        }

        if (prologueUi.avatarImage == null)
        {
            Transform avatarTransform = FindChildRecursive(transform, prologueUi.avatarName);
            if (avatarTransform != null)
            {
                prologueUi.avatarImage = avatarTransform.GetComponent<Image>();
            }
        }

        if (prologueUi.enterKey == null)
        {
            Transform enterKeyTransform = FindChildRecursive(transform, prologueUi.enterKeyName);
            if (enterKeyTransform != null)
            {
                prologueUi.enterKey = enterKeyTransform as RectTransform;
                if (prologueUi.enterKey == null)
                {
                    prologueUi.enterKey = enterKeyTransform.GetComponent<RectTransform>();
                }

                if (prologueUi.enterKey != null)
                {
                    prologueUi.enterKeyBaseScale = prologueUi.enterKey.localScale;
                }
            }
        }

        if (prologueUi.linesText == null)
        {
            Debug.LogError("Lines TMP_Text not found under " + name);
        }

        if (prologueUi.avatarImage == null)
        {
            Debug.LogError("Avatar Image not found under " + name);
        }

        if (prologueUi.enterKey == null)
        {
            Debug.LogError("EnterKey not found under " + name);
        }
    }

    public void PlayLines(string line, string avatar)
    {
        if (prologueUi == null)
        {
            prologueUi = new PrologueUi();
        }

        if (prologueUi.linesText == null || prologueUi.avatarImage == null || prologueUi.enterKey == null)
        {
            AutoFindReferences();
        }

        if (prologueUi.linesText == null)
        {
            return;
        }

        SetSubtitlePanelActive(true);
        prologueUi.linesText.text = line ?? string.Empty;
        ApplyAvatar(avatar);
    }

    public void PulseEnterKey()
    {
        if (prologueUi == null || prologueUi.enterKey == null)
        {
            AutoFindReferences();
        }

        if (prologueUi == null || prologueUi.enterKey == null)
        {
            return;
        }

        prologueUi.enterKey.localScale = prologueUi.enterKeyBaseScale * EnterKeyPulseScale;
    }

    public void ResetEnterKeyScale()
    {
        if (prologueUi == null || prologueUi.enterKey == null)
        {
            AutoFindReferences();
        }

        if (prologueUi != null && prologueUi.enterKey != null)
        {
            prologueUi.enterKey.localScale = prologueUi.enterKeyBaseScale;
        }
    }

    void SetSubtitlePanelActive(bool active)
    {
        Transform subtitlePanel = FindChildRecursive(transform, prologueUi.subtitlePanelName);
        if (subtitlePanel != null)
        {
            subtitlePanel.gameObject.SetActive(active);
        }
    }

    void ApplyAvatar(string avatar)
    {
        if (prologueUi.avatarImage == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(avatar))
        {
            prologueUi.avatarImage.sprite = null;
            prologueUi.avatarImage.color = new Color(1f, 1f, 1f, 0.15f);
            return;
        }

        Sprite sprite = Resources.Load<Sprite>(AvatarResourcePath + avatar);
        if (sprite == null)
        {
            Debug.LogError("Avatar sprite not found: " + AvatarResourcePath + avatar);
            return;
        }

        prologueUi.avatarImage.sprite = sprite;
        prologueUi.avatarImage.color = Color.white;
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
