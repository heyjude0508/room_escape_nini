 //using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ItemPuzzleImpl : MonoBehaviour, IItemPuzzle
{
    //public GameEvent gameEventAimStart;
    //public GameEvent gameEventAimEnd;
    //public GameEvent gameEventInteract;

    //public DOTweenAnimation dtAnim;

    [SerializeField] ItemPuzzle itemPuzzle;

    BagManagementImpl bag;

    void Awake()
    {
        if (itemPuzzle.animationSource == null)
        {
            itemPuzzle.animationSource = GetComponent<Animation>();
        }

        if (itemPuzzle.audioSource == null)
        {
            itemPuzzle.audioSource = GetComponent<AudioSource>();
        }

        if (itemPuzzle.puzzleCollider == null)
        {
            itemPuzzle.puzzleCollider = FindSolidCollider();
        }

        ResolvePlacedItemReference();

        if (!itemPuzzle.isSolved)
        {
            HidePlacedItem();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //dtAnim.DOPlay();
        bag = BagManagementImpl.Instance;

        if (itemPuzzle.isSolved)
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
        if (itemPuzzle.isSolved)
        {
            return string.Empty;
        }

        return CanSolve() ? itemPuzzle.placeDesc : itemPuzzle.puzzleDesc;
    }

    public void Interact()
    {
        if (itemPuzzle.isSolved)
        {
            return;
        }

        PlaceItem();
    }

    public void PlaceItem()
    {
        if (!CanSolve()) 
        {
            PlaySound(itemPuzzle.unsolvedSound);
            Debug.Log("Need to find the missing item!");
            return;
        }

        if (bag == null)
        {
            Debug.LogError("Cannot find the bag.");
            return;
        }

        if (!itemPuzzle.isSolved && bag.HasItem(itemPuzzle.itemId))
        {
            bag.RemoveItem(itemPuzzle.itemId);
            ShowPlacedItem();
            PlaySolveAnimation();
            PlaySound(itemPuzzle.solvedSound);
            MarkSolved();
            Debug.Log($"Place item {itemPuzzle.itemId} into {itemPuzzle.id} successfully.");
        }
    }

    public void ShowPlacedItem()
    {
        if (itemPuzzle.item == null)
        {
            return;
        }

        itemPuzzle.item.SetActive(true);
    }

    public void HidePlacedItem()
    {
        if (itemPuzzle.item == null)
        {
            return;
        }

        itemPuzzle.item.SetActive(false);
    }

    void ResolvePlacedItemReference()
    {
        if (itemPuzzle.item != null)
        {
            return;
        }

        Transform placedItem = transform.Find("PicArrivalDay");
        if (placedItem != null)
        {
            itemPuzzle.item = placedItem.gameObject;
        }
    }

    public bool CanSolve()
    {
        if (itemPuzzle.isSolved)
        {
            return true;
        }

        if (bag == null || string.IsNullOrEmpty(itemPuzzle.itemId))
        {
            return false;
        }

        return bag.HasItem(itemPuzzle.itemId);
    }

    public void PlaySolveAnimation()
    {
        if (itemPuzzle.animationSource == null)
        {
            Debug.LogWarning($"Animation is missing");
            return;
        }

        if (!string.IsNullOrEmpty(itemPuzzle.solveAnimationName))
        {
            itemPuzzle.animationSource.Play(itemPuzzle.solveAnimationName);
            return;
        }

        itemPuzzle.animationSource.Play();
    }

    public void PlaySound(AudioClip sound)
    {
        if (sound == null)
        {
            return;
        }

        if (itemPuzzle.audioSource != null)
        {
            itemPuzzle.audioSource.PlayOneShot(sound);
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
        itemPuzzle.isSolved = true;
        DisableInteractionColliders();
        DisableIconTips();

        if (itemPuzzle.puzzleCollider != null)
        {
            itemPuzzle.puzzleCollider.enabled = false;
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
        IconUiImpl[] iconTips = GetComponentsInChildren<IconUiImpl>(true);
        foreach (IconUiImpl iconTip in iconTips)
        {
            iconTip.enabled = false;
            iconTip.gameObject.SetActive(false);
        }
    }

}
