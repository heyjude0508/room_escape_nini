using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrologueUiImpl : MonoBehaviour, IPrologueUi
{
    const string SubtitlePanelName = "SubtitlePanel";
    const string LinesName = "Lines";
    const string KeyName = "Key";
    const string ContinueKeyText = "Enter";

    [SerializeField] PrologueUi prologueUi;
    [SerializeField] TMP_Text linesText;
    [SerializeField] RectTransform subtitlePanel;
    [SerializeField] Light cubeSpotlight;

    TMP_Text keyText;
    FirstPersonController firstPersonController;
    PlayerActionImpl playerAction;
    int currentLineIndex;
    bool isAdvancing;

    void Awake()
    {
        if (prologueUi == null)
        {
            prologueUi = new PrologueUi();
        }

        currentLineIndex = 0;
        isAdvancing = false;
        AutoFindReferences();
        if (cubeSpotlight != null)
        {
            cubeSpotlight.enabled = false;
        }

        ApplySubtitle();
    }

    void Update()
    {
        if (!IsSubtitleVisible() || isAdvancing || !WasContinuePressed())
        {
            return;
        }

        NextLine();
    }

    public void AutoFindReferences()
    {
        if (prologueUi == null)
        {
            prologueUi = new PrologueUi();
        }

        if (subtitlePanel == null)
        {
            Transform panelTransform = FindChildRecursive(transform, SubtitlePanelName);
            subtitlePanel = panelTransform != null
                ? panelTransform.GetComponent<RectTransform>()
                : null;
        }

        if (linesText == null)
        {
            Transform linesTransform = FindChildRecursive(transform, LinesName);
            linesText = linesTransform != null
                ? linesTransform.GetComponent<TMP_Text>()
                : null;
        }

        if (cubeSpotlight == null)
        {
            GameObject spotlightObject = GameObject.Find("CubeSpotlight");
            if (spotlightObject != null)
            {
                cubeSpotlight = spotlightObject.GetComponent<Light>();
            }
        }

        if (firstPersonController == null)
        {
            firstPersonController = FindObjectOfType<FirstPersonController>();
        }

        if (playerAction == null)
        {
            playerAction = FindObjectOfType<PlayerActionImpl>();
        }

        Transform keyTransform = FindChildRecursive(transform, KeyName);
        keyText = keyTransform != null
            ? keyTransform.GetComponent<TMP_Text>()
            : null;

        if (keyText != null)
        {
            keyText.text = ContinueKeyText;
            keyText.enableWordWrapping = false;
            keyText.overflowMode = TextOverflowModes.Overflow;
            if (linesText != null)
            {
                keyText.fontSize = linesText.fontSize;
            }
        }

        if (linesText != null)
        {
            linesText.enableWordWrapping = false;
            linesText.overflowMode = TextOverflowModes.Overflow;
            if (keyText != null)
            {
                keyText.fontSize = linesText.fontSize;
            }
        }

        if (linesText == null)
        {
            Debug.LogError("Lines TMP_Text not found under " + name);
        }

        if (subtitlePanel == null)
        {
            Debug.LogError("SubtitlePanel not found under " + name);
        }
    }

    public void ApplySubtitle()
    {
        if (prologueUi == null)
        {
            prologueUi = new PrologueUi();
        }

        EnsureLines();

        if (linesText == null || subtitlePanel == null)
        {
            AutoFindReferences();
        }

        if (linesText == null)
        {
            return;
        }

        if (prologueUi.prologueLines.lines.Count == 0)
        {
            ClearSubtitle();
            return;
        }

        if (currentLineIndex < 0 || currentLineIndex >= prologueUi.prologueLines.lines.Count)
        {
            ClearSubtitle();
            return;
        }

        PrologueLine line = prologueUi.prologueLines.lines[currentLineIndex];
        linesText.text = line != null ? (line.text ?? string.Empty) : string.Empty;
        RefreshSubtitlePanel();
        SetContinuePromptVisible(true);
        SyncPlayerInputLock();
    }

    public void NextLine()
    {
        if (isAdvancing || !IsSubtitleVisible())
        {
            return;
        }

        EnsureLines();
        if (prologueUi.prologueLines.lines.Count == 0)
        {
            return;
        }

        if (currentLineIndex < 0 || currentLineIndex >= prologueUi.prologueLines.lines.Count)
        {
            return;
        }

        StartCoroutine(CompleteCurrentLineThenAdvance());
    }

    IEnumerator CompleteCurrentLineThenAdvance()
    {
        isAdvancing = true;
        SetContinuePromptVisible(false);
        SyncPlayerInputLock();

        PrologueLine line = prologueUi.prologueLines.lines[currentLineIndex];
        if (line != null && line.onComplete != null)
        {
            for (int i = 0; i < line.onComplete.Count; i++)
            {
                yield return ExecuteCommand(line.onComplete[i]);
            }
        }

        if (currentLineIndex >= prologueUi.prologueLines.lines.Count - 1)
        {
            ClearSubtitle();
            isAdvancing = false;
            SyncPlayerInputLock();
            yield break;
        }

        currentLineIndex++;
        ApplySubtitle();
        isAdvancing = false;
        SyncPlayerInputLock();
    }

    IEnumerator ExecuteCommand(PrologueCommand command)
    {
        if (command == null || string.IsNullOrEmpty(command.id))
        {
            yield break;
        }

        switch (command.id)
        {
            case PrologueLines.CommandWait:
                yield return new WaitForSecondsRealtime(Mathf.Max(0f, command.value));
                break;

            case PrologueLines.CommandSpotlightOn:
                if (cubeSpotlight == null)
                {
                    AutoFindReferences();
                }

                if (cubeSpotlight != null)
                {
                    cubeSpotlight.enabled = true;
                    cubeSpotlight.intensity = 12f;
                }
                else
                {
                    Debug.LogError("CubeSpotlight not found for SpotlightOn command");
                }

                break;

            default:
                Debug.LogWarning("Unknown prologue command: " + command.id);
                break;
        }
    }

    void EnsureLines()
    {
        if (prologueUi == null)
        {
            prologueUi = new PrologueUi();
        }

        if (prologueUi.prologueLines == null || prologueUi.prologueLines.lines == null
            || prologueUi.prologueLines.lines.Count == 0)
        {
            prologueUi.prologueLines = new PrologueLines();
        }
    }

    bool WasContinuePressed()
    {
        return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
    }

    bool IsSubtitleVisible()
    {
        return linesText != null
            && !string.IsNullOrEmpty(linesText.text)
            && subtitlePanel != null
            && subtitlePanel.gameObject.activeSelf;
    }

    void ClearSubtitle()
    {
        if (linesText != null)
        {
            linesText.text = string.Empty;
        }

        SetContinuePromptVisible(false);
        RefreshSubtitlePanel();
        SyncPlayerInputLock();
    }

    void SyncPlayerInputLock()
    {
        SetPlayerInputLocked(isAdvancing || IsSubtitleVisible());
    }

    void SetPlayerInputLocked(bool locked)
    {
        if (firstPersonController == null || playerAction == null)
        {
            AutoFindReferences();
        }

        if (firstPersonController != null)
        {
            firstPersonController.playerCanMove = !locked;
            firstPersonController.cameraCanMove = !locked;

            if (locked)
            {
                Rigidbody rb = firstPersonController.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 velocity = rb.velocity;
                    rb.velocity = new Vector3(0f, velocity.y, 0f);
                }
            }
        }

        if (playerAction != null)
        {
            playerAction.enabled = !locked;
        }
    }

    void SetContinuePromptVisible(bool visible)
    {
        if (keyText == null)
        {
            return;
        }

        Transform icon = keyText.transform.parent;
        if (icon != null)
        {
            icon.gameObject.SetActive(visible);
        }
        else
        {
            keyText.gameObject.SetActive(visible);
        }
    }

    void RefreshSubtitlePanel()
    {
        if (subtitlePanel == null || linesText == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(linesText.text))
        {
            subtitlePanel.gameObject.SetActive(false);
            return;
        }

        if (!subtitlePanel.gameObject.activeSelf)
        {
            subtitlePanel.gameObject.SetActive(true);
        }

        Canvas.ForceUpdateCanvases();
        linesText.ForceMeshUpdate(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(subtitlePanel);
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
