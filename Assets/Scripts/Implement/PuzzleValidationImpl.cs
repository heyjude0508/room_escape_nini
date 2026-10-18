using UnityEngine;

public class PuzzleValidationImpl : MonoBehaviour, IPuzzleValidation
{
    [SerializeField] PuzzleValidation puzzleValidation;
    [SerializeField] PuzzleValidateUiImpl puzzleValidateUi;

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
            Debug.LogError("CombLockPanel / PuzzleValidateUiImpl not found for " + name);
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

        if (string.Equals(inputCode, puzzleValidation.validationCode, System.StringComparison.Ordinal))
        {
            PlaySolveAnimation();
            PlaySound(puzzleValidation.solvedSound);
            MarkSolved();
            if (puzzleValidateUi != null)
            {
                puzzleValidateUi.ShowCorrectThenClose();
            }

            Debug.Log($"Input {puzzleValidation.validationCode} correctly.");
            return;
        }

        PlaySound(puzzleValidation.unsolvedSound);
        if (puzzleValidateUi != null)
        {
            puzzleValidateUi.ShowErrorThenReset();
        }

        Debug.Log($"Wrong code: {inputCode}");
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

        GameObject panel = GameObject.Find("CombLockPanel");
        if (panel != null)
        {
            PuzzleValidateUiImpl ui = panel.GetComponent<PuzzleValidateUiImpl>();
            if (ui != null)
            {
                return ui;
            }
        }

        // CombLockPanel starts inactive; includeInactive finds it under Canvas.
        return FindObjectOfType<PuzzleValidateUiImpl>(true);
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

            // Prefer Lock ownership; if no Lock, fall back to tips owned by this object excluding keys.
            if (lockRoot != null)
            {
                if (!iconTip.IsOwnedBy(lockRoot))
                {
                    continue;
                }
            }
            else if (!iconTip.IsOwnedBy(transform))
            {
                continue;
            }

            iconTip.enabled = false;
            iconTip.gameObject.SetActive(false);
        }
    }
}
