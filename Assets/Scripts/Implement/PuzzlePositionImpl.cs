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

        if (!puzzlePosition.isSolved && bag.HasItem(puzzlePosition.itemId))
        {
            bag.RemoveItem(puzzlePosition.itemId);
            ShowPlacedItem();
            PlaySolveAnimation();
            PlaySound(puzzlePosition.solvedSound);
            MarkSolved();
            Debug.Log($"Place item {puzzlePosition.itemId} into {puzzlePosition.id} successfully.");
        }
    }

    public void ShowPlacedItem()
    {
        if (puzzlePosition.item == null)
        {
            return;
        }

        puzzlePosition.item.SetActive(true);
    }

    public void HidePlacedItem()
    {
        if (puzzlePosition.item == null)
        {
            return;
        }

        puzzlePosition.item.SetActive(false);
    }

    void ResolvePlacedItemReference()
    {
        if (puzzlePosition.item != null)
        {
            return;
        }

        Transform placedItem = transform.Find("PicArrivalDay");
        if (placedItem != null)
        {
            puzzlePosition.item = placedItem.gameObject;
        }
    }

    public bool CanSolve()
    {
        if (puzzlePosition.isSolved)
        {
            return true;
        }

        if (bag == null || string.IsNullOrEmpty(puzzlePosition.itemId))
        {
            return false;
        }

        return bag.HasItem(puzzlePosition.itemId);
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
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            if (!collider.isTrigger)
            {
                return collider;
            }
        }

        return null;
    }

    void MarkSolved()
    {
        puzzlePosition.isSolved = true;
        DisableInteractionColliders();
        DisableIconTips();

        if (puzzlePosition.puzzleCollider != null)
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
