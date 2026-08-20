 //using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class LockPuzzleImpl : MonoBehaviour, IPuzzleLock
{
    //public GameEvent gameEventAimStart;
    //public GameEvent gameEventAimEnd;
    //public GameEvent gameEventInteract;

    //public DOTweenAnimation dtAnim;

    [SerializeField] PuzzleLock puzzleLock;

    BagManagementImpl bag;

    void Awake()
    {
        if (puzzleLock.animationSource == null)
        {
            puzzleLock.animationSource = GetComponent<Animation>();
        }

        if (puzzleLock.audioSource == null)
        {
            puzzleLock.audioSource = GetComponent<AudioSource>();
        }

        if (puzzleLock.puzzleCollider == null)
        {
            puzzleLock.puzzleCollider = FindSolidCollider();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //dtAnim.DOPlay();
        bag = BagManagementImpl.Instance;
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
        if (puzzleLock.isSolved)
        {
            return string.Empty;
        }

        return CanSolve() ? puzzleLock.unlockDesc : puzzleLock.puzzleDesc;
    }

    public void Interact()
    {
        if (puzzleLock.isSolved)
        {
            return;
        }

        Unlock();
    }

    public void Unlock()
    {
        if (!CanSolve()) 
        {
            PlaySound(puzzleLock.unsolvedSound);
            Debug.Log("Need to find the key!");
            return;
        }

        if (puzzleLock.isSolved)
        {
            return;
        }

        if (!puzzleLock.bypassKeyRequirement)
        {
            if (bag == null)
            {
                Debug.LogError("Cannot find the bag.");
                return;
            }

            if (!bag.HasItem(puzzleLock.keyId))
            {
                PlaySound(puzzleLock.unsolvedSound);
                return;
            }

            bag.RemoveItem(puzzleLock.keyId);
            Debug.Log($"Unlock the lock {puzzleLock.id} using the key {puzzleLock.keyId} successfully.");
        }
        else
        {
            Debug.Log($"Unlock the lock {puzzleLock.id} successfully.");
        }

        PlaySolveAnimation();
        PlaySound(puzzleLock.solvedSound);
        MarkSolved();
    }

    public bool CanSolve()
    {
        if (puzzleLock.isSolved)
        {
            return true;
        }

        if (puzzleLock.bypassKeyRequirement)
        {
            return true;
        }

        if (bag == null || string.IsNullOrEmpty(puzzleLock.keyId))
        {
            return false;
        }

        return bag.HasItem(puzzleLock.keyId);
    }

    public void PlaySolveAnimation()
    {
        if (puzzleLock.animationSource == null)
        {
            Debug.LogWarning($"Animation is missing");
            return;
        }

        if (!string.IsNullOrEmpty(puzzleLock.solveAnimationName))
        {
            puzzleLock.animationSource.Play(puzzleLock.solveAnimationName);
            return;
        }

        puzzleLock.animationSource.Play();
    }

    public void PlaySound(AudioClip sound)
    {
        if (sound == null)
        {
            return;
        }

        if (puzzleLock.audioSource != null)
        {
            puzzleLock.audioSource.PlayOneShot(sound);
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
        puzzleLock.isSolved = true;
        DisableInteractionColliders();
        DisableIconTips();

        if (puzzleLock.puzzleCollider != null)
        {
            puzzleLock.puzzleCollider.enabled = false;
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
