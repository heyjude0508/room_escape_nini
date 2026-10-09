using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlotUiImpl : MonoBehaviour, IPlotUi
{
    const string AvatarResourcePath = "UI/Avatar/";
    const float EnterKeyPulseScale = 1.2f;

    PlotUi plotUi;
    FirstPersonController firstPersonController;

    void Awake()
    {
        if (plotUi == null)
        {
            plotUi = new PlotUi();
        }

        AutoFindReferences();
        SetSubtitlePanelActive(false);
        SetTipPanelActive(false);
    }

    public void AutoFindReferences()
    {
        if (plotUi == null)
        {
            plotUi = new PlotUi();
        }

        if (firstPersonController == null)
        {
            firstPersonController = FindObjectOfType<FirstPersonController>();
        }

        if (string.IsNullOrEmpty(plotUi.linesName))
        {
            plotUi.linesName = "Lines";
        }

        if (string.IsNullOrEmpty(plotUi.subtitlePanelName))
        {
            plotUi.subtitlePanelName = "SubtitlePanel";
        }

        if (string.IsNullOrEmpty(plotUi.tipPanelName))
        {
            plotUi.tipPanelName = "TipPanel";
        }

        if (string.IsNullOrEmpty(plotUi.avatarName))
        {
            plotUi.avatarName = "Avatar";
        }

        if (string.IsNullOrEmpty(plotUi.enterKeyName))
        {
            plotUi.enterKeyName = "EnterKey";
        }

        if (plotUi.linesText == null)
        {
            Transform linesTransform = FindChildRecursive(transform, plotUi.linesName);
            if (linesTransform != null)
            {
                plotUi.linesText = linesTransform.GetComponent<TMP_Text>();
            }
        }

        if (plotUi.avatarImage == null)
        {
            Transform avatarTransform = FindChildRecursive(transform, plotUi.avatarName);
            if (avatarTransform != null)
            {
                plotUi.avatarImage = avatarTransform.GetComponent<Image>();
            }
        }

        if (plotUi.enterKey == null)
        {
            Transform enterKeyTransform = FindChildRecursive(transform, plotUi.enterKeyName);
            if (enterKeyTransform != null)
            {
                plotUi.enterKey = enterKeyTransform as RectTransform;
                if (plotUi.enterKey == null)
                {
                    plotUi.enterKey = enterKeyTransform.GetComponent<RectTransform>();
                }

                if (plotUi.enterKey != null)
                {
                    plotUi.enterKeyBaseScale = plotUi.enterKey.localScale;
                }
            }
        }

        if (plotUi.linesText == null)
        {
            Debug.LogError("Lines TMP_Text not found under " + name);
        }

        if (plotUi.avatarImage == null)
        {
            Debug.LogError("Avatar Image not found under " + name);
        }

        if (plotUi.enterKey == null)
        {
            Debug.LogError("EnterKey not found under " + name);
        }

        if (firstPersonController == null)
        {
            Debug.LogError("FirstPersonController not found in scene.");
        }
    }

    public void PlayLines(string avatar, string line)
    {
        if (plotUi == null)
        {
            plotUi = new PlotUi();
        }

        if (plotUi.linesText == null || plotUi.avatarImage == null || plotUi.enterKey == null || firstPersonController == null)
        {
            AutoFindReferences();
        }

        if (plotUi.linesText == null)
        {
            return;
        }

        SetPlayerLocked(true);
        SetSubtitlePanelActive(true);
        plotUi.linesText.text = line ?? string.Empty;
        ApplyAvatar(avatar);
    }

    public void PlayTip()
    {
        if (plotUi == null)
        {
            plotUi = new PlotUi();
        }

        if (string.IsNullOrEmpty(plotUi.tipPanelName))
        {
            AutoFindReferences();
        }

        Transform tipPanel = FindChildRecursive(transform, plotUi.tipPanelName);
        if (tipPanel == null)
        {
            Debug.LogError("TipPanel not found under " + name);
            return;
        }

        tipPanel.gameObject.SetActive(true);
    }

    public void HideTip()
    {
        if (plotUi == null)
        {
            plotUi = new PlotUi();
        }

        SetTipPanelActive(false);
    }

    public void PulseEnterKey()
    {
        if (plotUi == null || plotUi.enterKey == null)
        {
            AutoFindReferences();
        }

        if (plotUi == null || plotUi.enterKey == null)
        {
            return;
        }

        plotUi.enterKey.localScale = plotUi.enterKeyBaseScale * EnterKeyPulseScale;
    }

    public void ResetEnterKeyScale()
    {
        if (plotUi == null || plotUi.enterKey == null)
        {
            AutoFindReferences();
        }

        if (plotUi != null && plotUi.enterKey != null)
        {
            plotUi.enterKey.localScale = plotUi.enterKeyBaseScale;
        }
    }

    public bool IsEnterPressed()
    {
        return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
    }

    public void HideLines()
    {
        if (plotUi == null)
        {
            plotUi = new PlotUi();
        }

        ResetEnterKeyScale();
        SetSubtitlePanelActive(false);
        SetPlayerLocked(false);
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
    }

    void SetSubtitlePanelActive(bool active)
    {
        Transform subtitlePanel = FindChildRecursive(transform, plotUi.subtitlePanelName);
        if (subtitlePanel != null)
        {
            subtitlePanel.gameObject.SetActive(active);
        }
    }

    void SetTipPanelActive(bool active)
    {
        if (plotUi == null || string.IsNullOrEmpty(plotUi.tipPanelName))
        {
            return;
        }

        Transform tipPanel = FindChildRecursive(transform, plotUi.tipPanelName);
        if (tipPanel != null)
        {
            tipPanel.gameObject.SetActive(active);
        }
    }

    void ApplyAvatar(string avatar)
    {
        if (plotUi.avatarImage == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(avatar))
        {
            plotUi.avatarImage.sprite = null;
            plotUi.avatarImage.color = new Color(1f, 1f, 1f, 0.15f);
            return;
        }

        Sprite sprite = Resources.Load<Sprite>(AvatarResourcePath + avatar);
        if (sprite == null)
        {
            Debug.LogError("Avatar sprite not found: " + AvatarResourcePath + avatar);
            return;
        }

        plotUi.avatarImage.sprite = sprite;
        plotUi.avatarImage.color = Color.white;
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
