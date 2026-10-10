 //using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PuzzlePositionImpl : MonoBehaviour, IPuzzlePosition
{
    //public GameEvent gameEventAimStart;
    //public GameEvent gameEventAimEnd;
    //public GameEvent gameEventInteract;

    //public DOTweenAnimation dtAnim;

    [SerializeField] PuzzlePosition puzzlePosition;

    BagManagementImpl bag;

    void Awake()
    {
        if (puzzlePosition.animationSource == null)
        {
            puzzlePosition.animationSource = GetComponent<Animation>();
        }

        if (puzzlePosition.audioSource == null)
        {
            puzzlePosition.audioSource = GetComponent<AudioSource>();
        }

        if (puzzlePosition.puzzleCollider == null)
        {
            puzzlePosition.puzzleCollider = FindSolidCollider();
        }

        ResolvePlacedItemReference();

        if (!puzzlePosition.isSolved)
        {
            HidePlacedItem();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //dtAnim.DOPlay();
        bag = BagManagementImpl.Instance;

        if (puzzlePosition.isSolved)
        {
            ShowPlacedItem();
        }
        else
        {
            HidePlacedItem();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EventAimStart()
    {
        //gameEventAimStart.Raise();
        ActPanelKeyUi.SetCanInteract(CanSolve());
    }

    public void EventAimEnd()
    {
        //gameEventAimEnd.Raise();
    }

    public void EventInteract()
    {
        //gameEventInteract.Raise();
    }

    public string GetDescription()
    {
        if (puzzlePosition.isSolved)
        {
            return string.Empty;
        }

        return CanSolve() ? puzzlePosition.placeDesc : puzzlePosition.puzzleDesc;
    }

    public void Interact()
    {
        if (puzzlePosition.isSolved)
        {
            return;
        }

        PlaceItem();
    }

    public void PlaceItem()
    {
        if (!CanSolve()) 
        {
            PlaySound(puzzlePosition.unsolvedSound);
            Debug.Log("Need to find the missing item!");
            return;
        }

        if (bag == null)
        {
            Debug.LogError("Cannot find the bag.");
            return;
        }

        if (!puzzlePosition.isSolved && bag.HasItem(puzzlePosition.socketId))
        {
            bag.RemoveItem(puzzlePosition.socketId);
            ShowPlacedItem();
            PlaySolveAnimation();
            PlaySound(puzzlePosition.solvedSound);
            MarkSolved();
            // MarkSolved disables socket tips/colliders; keep the placed item readable/interactable.
            ShowPlacedItem();
            EnableOriginalItemColliders();
            Debug.Log($"Place item {puzzlePosition.socketId} into {puzzlePosition.id} successfully.");
        }
    }

    public void ShowPlacedItem()
    {
        SetPlacedItemActive(true);
    }

    public void HidePlacedItem()
    {
        SetPlacedItemActive(false);
    }

    void SetPlacedItemActive(bool active)
    {
        if (puzzlePosition.originalItem == null)
        {
            return;
        }

        Transform itemRoot = puzzlePosition.originalItem.transform;

        // IconTips under Interactions detach to scene root; parent SetActive won't reach them.
        SetOwnedIconTipsActive(itemRoot, active);

        for (int i = 0; i < itemRoot.childCount; i++)
        {
            itemRoot.GetChild(i).gameObject.SetActive(active);
        }

        puzzlePosition.originalItem.SetActive(active);
    }

    void SetOwnedIconTipsActive(Transform owner, bool active)
    {
        IconUiImpl[] iconTips = FindObjectsOfType<IconUiImpl>(true);
        foreach (IconUiImpl iconTip in iconTips)
        {
            if (!iconTip.IsOwnedBy(owner))
            {
                continue;
            }

            iconTip.enabled = active;
            iconTip.gameObject.SetActive(active);
        }
    }

    void ResolvePlacedItemReference()
    {
        if (puzzlePosition.originalItem != null)
        {
            return;
        }

        Transform placedItem = transform.Find(puzzlePosition.id);
        if (placedItem != null)
        {
            puzzlePosition.originalItem = placedItem.gameObject;
        }
    }

    public bool CanSolve()
    {
        if (puzzlePosition.isSolved)
        {
            return true;
        }

        if (bag == null || string.IsNullOrEmpty(puzzlePosition.socketId))
        {
            return false;
        }

        return bag.HasItem(puzzlePosition.socketId);
    }

    public void PlaySolveAnimation()
    {
        if (puzzlePosition.animationSource == null)
        {
            Debug.LogWarning($"Animation is missing");
            return;
        }

        if (!string.IsNullOrEmpty(puzzlePosition.solveAnimationName))
        {
            puzzlePosition.animationSource.Play(puzzlePosition.solveAnimationName);
            return;
        }

        puzzlePosition.animationSource.Play();
    }

    public void PlaySound(AudioClip sound)
    {
        if (sound == null)
        {
            return;
        }

        if (puzzlePosition.audioSource != null)
        {
            puzzlePosition.audioSource.PlayOneShot(sound);
            return;
        }

        AudioSource.PlayClipAtPoint(sound, transform.position);
    }

    public Collider FindSolidCollider()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(true);
        foreach (Collider collider in colliders)
        {
            if (collider.isTrigger || IsUnderOriginalItem(collider.transform))
            {
                continue;
            }

            return collider;
        }

        return null;
    }

    void MarkSolved()
    {
        puzzlePosition.isSolved = true;
        DisableInteractionColliders();
        DisableIconTips();

        if (puzzlePosition.puzzleCollider != null && !IsUnderOriginalItem(puzzlePosition.puzzleCollider.transform))
        {
            puzzlePosition.puzzleCollider.enabled = false;
        }

        enabled = false;
    }

    void DisableInteractionColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(true);
        foreach (Collider collider in colliders)
        {
            if (IsUnderOriginalItem(collider.transform))
            {
                continue;
            }

            collider.enabled = false;
        }
    }

    void EnableOriginalItemColliders()
    {
        if (puzzlePosition.originalItem == null)
        {
            return;
        }

        Collider[] colliders = puzzlePosition.originalItem.GetComponentsInChildren<Collider>(true);
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
    }

    void DisableIconTips()
    {
        IconUiImpl[] iconTips = FindObjectsOfType<IconUiImpl>(true);
        foreach (IconUiImpl iconTip in iconTips)
        {
            // Keep tips that belong to the placed photo (story interact), not the empty socket.
            if (puzzlePosition.originalItem != null &&
                iconTip.IsOwnedBy(puzzlePosition.originalItem.transform))
            {
                continue;
            }

            if (!iconTip.IsOwnedBy(transform))
            {
                continue;
            }

            iconTip.enabled = false;
            iconTip.gameObject.SetActive(false);
        }
    }

    bool IsUnderOriginalItem(Transform target)
    {
        if (puzzlePosition.originalItem == null || target == null)
        {
            return false;
        }

        Transform itemRoot = puzzlePosition.originalItem.transform;
        return target == itemRoot || target.IsChildOf(itemRoot);
    }

}
