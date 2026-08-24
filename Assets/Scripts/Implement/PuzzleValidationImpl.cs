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
            Debug.LogError("InputCanvas / PuzzleValidateUiImpl not found under " + name);
            return;
        }

        puzzleValidateUi.Show();
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
        return GetComponentInChildren<PuzzleValidateUiImpl>(true);
    }

    void MarkSolved()
    {
        puzzleValidation.isSolved = true;
        DisableInteractionColliders();
        DisableIconTips();

        if (puzzleValidation.puzzleCollider != null)
        {
            puzzleValidation.puzzleCollider.enabled = false;
        }

        enabled = false;
    }

    void DisableInteractionColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(true);
        foreach (Collider collider in colliders)
        {
            // Keep UI raycasts on InputCanvas intact.
            if (collider.GetComponentInParent<Canvas>() != null)
            {
                continue;
            }

            collider.enabled = false;
        }
    }

    void DisableIconTips()
    {
        IconUiImpl[] iconTips = FindObjectsOfType<IconUiImpl>(true);
        foreach (IconUiImpl iconTip in iconTips)
        {
            if (!iconTip.IsOwnedBy(transform))
            {
                continue;
            }

            iconTip.enabled = false;
            iconTip.gameObject.SetActive(false);
        }
    }
}
