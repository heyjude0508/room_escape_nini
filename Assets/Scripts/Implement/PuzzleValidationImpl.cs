using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PuzzleValidationImpl : MonoBehaviour, IPuzzleValidation
{
    public const string DynamicWebsitePasswordCode = "dynamic";

    const string ParentDad = "Dad";
    const string ParentMom = "Mom";
    static readonly Regex ParentPairGroupRegex = new Regex(
        @"\(([^)]*)\)",
        RegexOptions.Compiled);
    [SerializeField] PuzzleValidation puzzleValidation;
    [SerializeField] PuzzleValidateUiImpl puzzleValidateUi;
    [SerializeField] string validateUiPanelName = "CombLockPanel";

    readonly HashSet<string> acceptedBagParents = new HashSet<string>();

    public bool HasAcceptedBagParent(string parent)
    {
        return !string.IsNullOrEmpty(parent) && acceptedBagParents.Contains(parent);
    }

    public string PuzzleId => puzzleValidation != null ? puzzleValidation.id : string.Empty;

    /// <summary>
    /// When Validation Code is "dynamic", password is set by website Snake score (sid session).
    /// </summary>
    public bool UsesDynamicWebsitePassword =>
        puzzleValidation != null
        && string.Equals(
            puzzleValidation.validationCode,
            DynamicWebsitePasswordCode,
            System.StringComparison.OrdinalIgnoreCase);

    public void SetValidationCode(string code)
    {
        if (puzzleValidation == null)
        {
            puzzleValidation = new PuzzleValidation();
        }

        puzzleValidation.validationCode = code ?? string.Empty;
    }

    void Awake()
    {
        if (puzzleValidation == null)
        {
            puzzleValidation = new PuzzleValidation();
        }

        if (puzzleValidation.animationSource == null)
        {
            puzzleValidation.animationSource = GetComponent<Animation>();
        }

        if (puzzleValidation.audioSource == null)
        {
            puzzleValidation.audioSource = GetComponent<AudioSource>();
        }

        if (puzzleValidation.puzzleCollider == null)
        {
            puzzleValidation.puzzleCollider = FindSolidCollider();
        }

        if (puzzleValidateUi == null)
        {
            puzzleValidateUi = FindPuzzleValidateUi();
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
        if (puzzleValidation == null || puzzleValidation.isSolved)
        {
            return string.Empty;
        }

        return puzzleValidation.validationDesc;
    }

    public void Interact()
    {
        if (puzzleValidation == null || puzzleValidation.isSolved)
        {
            return;
        }

        OpenInputCanvas();
    }

    public void OpenInputCanvas()
    {
        if (puzzleValidation == null || puzzleValidation.isSolved)
        {
            return;
        }

        if (puzzleValidateUi == null)
        {
            puzzleValidateUi = FindPuzzleValidateUi();
        }

        if (puzzleValidateUi == null)
        {
            Debug.LogError(validateUiPanelName + " / PuzzleValidateUiImpl not found for " + name);
            return;
        }

        puzzleValidateUi.ShowFor(this);
    }

    public void ValidateCode(string inputCode)
    {
        if (puzzleValidation == null || puzzleValidation.isSolved)
        {
            return;
        }

        string expected = puzzleValidation.validationCode;
        if (string.IsNullOrEmpty(expected)
            || string.Equals(expected, DynamicWebsitePasswordCode, System.StringComparison.OrdinalIgnoreCase))
        {
            // Still "dynamic" means Snake score has not been applied yet.
            RejectCode(inputCode);
            return;
        }

        // Exact match (comb lock / legacy).
        if (string.Equals(inputCode, expected, System.StringComparison.Ordinal))
        {
            CompletePuzzleSolved();
            return;
        }

        // Rolling cart: Dad / Mom submitted independently.
        if (TryParseParentItemInput(inputCode, out string parent, out _)
            && MatchesParentPairBagCode(inputCode, expected))
        {
            if (HasAcceptedBagParent(parent))
            {
                if (puzzleValidateUi != null)
                {
                    puzzleValidateUi.ShowBagParentAlreadyAccepted(parent);
                }

                return;
            }

            if (puzzleValidateUi != null)
            {
                // Capture locked slot visuals before removing items from the bag.
                puzzleValidateUi.NotifyBagParentAccepted(parent);
                puzzleValidateUi.ConsumeSelectedBagItems();
            }

            acceptedBagParents.Add(parent);

            if (AreAllBagParentsAccepted(expected))
            {
                CompletePuzzleSolved();
            }
            else
            {
                PlaySound(puzzleValidation.solvedSound);
                if (puzzleValidateUi != null)
                {
                    puzzleValidateUi.ShowBagParentAcceptedStayOpen(parent);
                }
            }

            Debug.Log($"Accepted bag parent '{parent}' for {name}.");
            return;
        }

        RejectCode(inputCode);
    }

    void CompletePuzzleSolved()
    {
        PlaySolveAnimation();
        PlaySound(puzzleValidation.solvedSound);
        MarkSolved();
        if (puzzleValidateUi != null)
        {
            puzzleValidateUi.ShowCorrectThenClose();
        }

        if (IsRollingCartPuzzle())
        {
            HouseChildSequencer sequencer = FindObjectOfType<HouseChildSequencer>(true);
            if (sequencer != null)
            {
                sequencer.NotifyRollingCartSolved();
            }
            else
            {
                Debug.LogError("HouseChildSequencer not found for RollingCart solve.");
            }
        }

        Debug.Log($"Input {puzzleValidation.validationCode} correctly.");
    }

    bool IsRollingCartPuzzle()
    {
        if (!string.IsNullOrWhiteSpace(validateUiPanelName)
            && string.Equals(validateUiPanelName, "RollingCartPanel", System.StringComparison.Ordinal))
        {
            return true;
        }

        return puzzleValidation != null
            && string.Equals(puzzleValidation.id, "RollingCart", System.StringComparison.Ordinal);
    }

    void RejectCode(string inputCode)
    {
        PlaySound(puzzleValidation.unsolvedSound);
        if (puzzleValidateUi != null)
        {
            puzzleValidateUi.ShowErrorThenReset();
        }

        Debug.Log($"Wrong code: {inputCode}");
    }

    bool AreAllBagParentsAccepted(string validationCode)
    {
        if (!TryParseParentPairValidation(validationCode, out Dictionary<string, string[][]> byParent)
            || byParent == null
            || byParent.Count == 0)
        {
            return false;
        }

        foreach (string parent in byParent.Keys)
        {
            if (!acceptedBagParents.Contains(parent))
            {
                return false;
            }
        }

        return true;
    }

    static bool MatchesParentPairBagCode(string inputCode, string validationCode)
    {
        if (string.IsNullOrWhiteSpace(inputCode) || string.IsNullOrWhiteSpace(validationCode))
        {
            return false;
        }

        if (!TryParseParentItemInput(inputCode, out string parent, out string[] selectedIds))
        {
            return false;
        }

        if (!TryParseParentPairValidation(validationCode, out Dictionary<string, string[][]> byParent))
        {
            return false;
        }

        if (!byParent.TryGetValue(parent, out string[][] pairs) || pairs == null)
        {
            return false;
        }

        if (selectedIds == null || selectedIds.Length != pairs.Length)
        {
            return false;
        }

        // Collect non-empty unique selected ids (order does not matter).
        List<string> selected = new List<string>(selectedIds.Length);
        HashSet<string> seen = new HashSet<string>();
        for (int i = 0; i < selectedIds.Length; i++)
        {
            string id = selectedIds[i] ?? string.Empty;
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            if (!seen.Add(id))
            {
                return false;
            }

            selected.Add(id);
        }

        if (selected.Count != pairs.Length)
        {
            return false;
        }

        // Unordered: assign each parenthesis exactly one selected id that belongs to it.
        bool[] usedSelected = new bool[selected.Count];
        return TryAssignPairsToSelected(pairs, selected, 0, usedSelected);
    }

    static bool TryAssignPairsToSelected(
        string[][] pairs,
        List<string> selected,
        int pairIndex,
        bool[] usedSelected)
    {
        if (pairIndex >= pairs.Length)
        {
            return true;
        }

        string[] pair = pairs[pairIndex];
        if (pair == null || pair.Length < 2)
        {
            return false;
        }

        string optionA = pair[0] ?? string.Empty;
        string optionB = pair[1] ?? string.Empty;

        for (int i = 0; i < selected.Count; i++)
        {
            if (usedSelected[i])
            {
                continue;
            }

            string id = selected[i];
            bool matchA = string.Equals(id, optionA, System.StringComparison.Ordinal);
            bool matchB = string.Equals(id, optionB, System.StringComparison.Ordinal);
            if (!matchA && !matchB)
            {
                continue;
            }

            // This parenthesis consumes exactly this one id.
            usedSelected[i] = true;
            if (TryAssignPairsToSelected(pairs, selected, pairIndex + 1, usedSelected))
            {
                return true;
            }

            usedSelected[i] = false;
        }

        return false;
    }

    static bool TryParseParentItemInput(string inputCode, out string parent, out string[] selectedIds)
    {
        parent = null;
        selectedIds = null;

        int sep = inputCode.IndexOf('|');
        if (sep <= 0 || sep >= inputCode.Length - 1)
        {
            return false;
        }

        parent = inputCode.Substring(0, sep).Trim();
        if (!string.Equals(parent, ParentDad, System.StringComparison.Ordinal)
            && !string.Equals(parent, ParentMom, System.StringComparison.Ordinal))
        {
            return false;
        }

        string itemsPart = inputCode.Substring(sep + 1);
        selectedIds = itemsPart.Split(',');
        for (int i = 0; i < selectedIds.Length; i++)
        {
            selectedIds[i] = selectedIds[i] != null ? selectedIds[i].Trim() : string.Empty;
        }

        return selectedIds.Length > 0;
    }

    static bool TryParseParentPairValidation(
        string validationCode,
        out Dictionary<string, string[][]> byParent)
    {
        byParent = new Dictionary<string, string[][]>();
        string[] sections = validationCode.Split('|');
        if (sections == null || sections.Length == 0)
        {
            return false;
        }

        for (int s = 0; s < sections.Length; s++)
        {
            string section = sections[s] != null ? sections[s].Trim() : string.Empty;
            if (string.IsNullOrEmpty(section))
            {
                continue;
            }

            int colon = section.IndexOf(':');
            if (colon <= 0)
            {
                return false;
            }

            string parent = section.Substring(0, colon).Trim();
            if (!string.Equals(parent, ParentDad, System.StringComparison.Ordinal)
                && !string.Equals(parent, ParentMom, System.StringComparison.Ordinal))
            {
                return false;
            }

            string pairsPart = section.Substring(colon + 1);
            MatchCollection matches = ParentPairGroupRegex.Matches(pairsPart);
            if (matches == null || matches.Count == 0)
            {
                return false;
            }

            string[][] pairs = new string[matches.Count][];
            for (int i = 0; i < matches.Count; i++)
            {
                string inner = matches[i].Groups[1].Value;
                string[] parts = inner.Split(',');
                string a = parts.Length > 0 && parts[0] != null ? parts[0].Trim() : string.Empty;
                string b = parts.Length > 1 && parts[1] != null ? parts[1].Trim() : string.Empty;
                if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
                {
                    return false;
                }

                pairs[i] = new[] { a, b };
            }

            byParent[parent] = pairs;
        }

        return byParent.Count > 0;
    }

    public void PlaySolveAnimation()
    {
        if (puzzleValidation.animationSource == null)
        {
            Debug.LogWarning("Animation is missing");
            return;
        }

        if (!string.IsNullOrEmpty(puzzleValidation.solveAnimationName))
        {
            puzzleValidation.animationSource.Play(puzzleValidation.solveAnimationName);
            return;
        }

        puzzleValidation.animationSource.Play();
    }

    public void PlaySound(AudioClip sound)
    {
        if (sound == null)
        {
            return;
        }

        if (puzzleValidation.audioSource != null)
        {
            puzzleValidation.audioSource.PlayOneShot(sound);
            return;
        }

        AudioSource.PlayClipAtPoint(sound, transform.position);
    }

    public Collider FindSolidCollider()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(true);
        foreach (Collider collider in colliders)
        {
            if (!collider.isTrigger)
            {
                return collider;
            }
        }

        // Fall back to trigger InteractZone so raycast targets still resolve.
        foreach (Collider collider in colliders)
        {
            if (collider.isTrigger)
            {
                return collider;
            }
        }

        return null;
    }

    PuzzleValidateUiImpl FindPuzzleValidateUi()
    {
        PuzzleValidateUiImpl local = GetComponentInChildren<PuzzleValidateUiImpl>(true);
        if (local != null)
        {
            return local;
        }

        if (!string.IsNullOrWhiteSpace(validateUiPanelName))
        {
            PuzzleValidateUiImpl named = FindValidateUiByPanelName(validateUiPanelName);
            if (named != null)
            {
                return named;
            }
        }

        // Panels often start inactive; includeInactive finds them under Canvas.
        return FindObjectOfType<PuzzleValidateUiImpl>(true);
    }

    static PuzzleValidateUiImpl FindValidateUiByPanelName(string panelName)
    {
        // GameObject.Find skips inactive objects; panels start disabled.
        PuzzleValidateUiImpl[] panels = FindObjectsOfType<PuzzleValidateUiImpl>(true);
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null && panels[i].gameObject.name == panelName)
            {
                return panels[i];
            }
        }

        return null;
    }

    void MarkSolved()
    {
        puzzleValidation.isSolved = true;
        DisablePuzzleInteractionColliders();
        DisableSelfBoxColliders();
        EnableInactiveContents();
        DisablePuzzleIconTips();

        if (puzzleValidation.puzzleCollider != null)
        {
            puzzleValidation.puzzleCollider.enabled = false;
        }

        enabled = false;
    }

    // Turn off lock / InteractZone colliders only — keep ItemKey pickups usable.
    void DisablePuzzleInteractionColliders()
    {
        Transform lockRoot = transform.Find("Lock");
        if (lockRoot != null)
        {
            DisableCollidersUnder(lockRoot, keepItemKeys: true);
        }

        Collider[] selfColliders = GetComponents<Collider>();
        for (int i = 0; i < selfColliders.Length; i++)
        {
            if (selfColliders[i] != null && selfColliders[i].isTrigger)
            {
                selfColliders[i].enabled = false;
            }
        }
    }

    void DisableCollidersUnder(Transform root, bool keepItemKeys)
    {
        Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            if (collider == null)
            {
                continue;
            }

            if (collider.GetComponentInParent<Canvas>() != null)
            {
                continue;
            }

            if (keepItemKeys && collider.GetComponentInParent<ItemKeyImpl>() != null)
            {
                continue;
            }

            collider.enabled = false;
        }
    }

    // PuzzleValidationImpl lives on GiftBox; disable this object's BoxColliders after unlock.
    void DisableSelfBoxColliders()
    {
        BoxCollider[] boxColliders = GetComponents<BoxCollider>();
        for (int i = 0; i < boxColliders.Length; i++)
        {
            if (boxColliders[i] != null)
            {
                boxColliders[i].enabled = false;
            }
        }
    }

    // Reveal inactive rewards under this object (e.g. DoorSideKey_L) and enable their colliders.
    void EnableInactiveContents()
    {
        EnableInactiveRecursive(transform);
    }

    void EnableInactiveRecursive(Transform root)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);

            if (child.name == "KeyBlock" || child.name == "Lock")
            {
                continue;
            }

            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
                EnableCollidersUnder(child);
            }

            EnableInactiveRecursive(child);
        }
    }

    void EnableCollidersUnder(Transform root)
    {
        Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
            {
                colliders[i].enabled = true;
            }
        }
    }

    // Only hide lock tips — never the key's IconTip under DoorSideKey_L.
    void DisablePuzzleIconTips()
    {
        Transform lockRoot = transform.Find("Lock");
        IconUiImpl[] iconTips = FindObjectsOfType<IconUiImpl>(true);
        foreach (IconUiImpl iconTip in iconTips)
        {
            if (iconTip == null)
            {
                continue;
            }

            if (iconTip.GetComponentInParent<ItemKeyImpl>() != null)
            {
                continue;
            }

            // Prefer Lock ownership; if no Lock, tips owned by this object or its root
            // (detached IconTips keep ownerRoot under RollingCart_01 while MainCart holds the puzzle).
            if (lockRoot != null)
            {
                if (!iconTip.IsOwnedBy(lockRoot))
                {
                    continue;
                }
            }
            else if (!iconTip.IsOwnedBy(transform)
                     && (transform.root == null || !iconTip.IsOwnedBy(transform.root)))
            {
                continue;
            }

            iconTip.enabled = false;
            iconTip.gameObject.SetActive(false);
        }
    }
}
