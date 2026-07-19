 //using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class LockPuzzleImpl : MonoBehaviour, ILockPuzzle
{
    //public GameEvent gameEventAimStart;
    //public GameEvent gameEventAimEnd;
    //public GameEvent gameEventInteract;

    //public DOTweenAnimation dtAnim;

    [SerializeField] LockPuzzle lockPuzzle;

    BagManagementImpl bag;

    void Awake()
    {
        if (lockPuzzle.animationSource == null)
        {
            lockPuzzle.animationSource = GetComponent<Animation>();
        }

        if (lockPuzzle.audioSource == null)
        {
            lockPuzzle.audioSource = GetComponent<AudioSource>();
        }

        if (lockPuzzle.puzzleCollider == null)
        {
            lockPuzzle.puzzleCollider = FindSolidCollider();
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
        if (lockPuzzle.isSolved)
        {
            return string.Empty;
        }

        return CanSolve() ? lockPuzzle.unlockDesc : lockPuzzle.puzzleDesc;
    }

    public void Interact()
    {
        if (lockPuzzle.isSolved)
        {
            return;
        }

        Unlock();
    }

    public void Unlock()
    {
        if (!CanSolve()) 
        {
            PlaySound(lockPuzzle.unsolvedSound);
            Debug.Log("Need to find the key!");
            return;
        }

        if (bag == null)
        {
            Debug.LogError("Cannot find the bag.");
            return;
        }

        if (!lockPuzzle.isSolved && bag.HasItem(lockPuzzle.keyId))
        {
            bag.RemoveItem(lockPuzzle.keyId);
            PlaySolveAnimation();
            PlaySound(lockPuzzle.solvedSound);
            MarkSolved();
            Debug.Log($"Unlock the lock {lockPuzzle.id} using the key {lockPuzzle.keyId} successfully.");
        }
    }

    public bool CanSolve()
    {
        if (lockPuzzle.isSolved)
        {
            return true;
        }

        if (bag == null || string.IsNullOrEmpty(lockPuzzle.keyId))
        {
            return false;
        }

        return bag.HasItem(lockPuzzle.keyId);
    }

    public void PlaySolveAnimation()
    {
        if (lockPuzzle.animationSource == null)
        {
            Debug.LogWarning($"Animation is missing");
            return;
        }

        if (!string.IsNullOrEmpty(lockPuzzle.solveAnimationName))
        {
            lockPuzzle.animationSource.Play(lockPuzzle.solveAnimationName);
            return;
        }

        lockPuzzle.animationSource.Play();
    }

    public void PlaySound(AudioClip sound)
    {
        if (sound == null)
        {
            return;
        }

        if (lockPuzzle.audioSource != null)
        {
            lockPuzzle.audioSource.PlayOneShot(sound);
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
        lockPuzzle.isSolved = true;
        DisableInteractionColliders();
        DisableIconTips();

        if (lockPuzzle.puzzleCollider != null)
        {
            lockPuzzle.puzzleCollider.enabled = false;
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
