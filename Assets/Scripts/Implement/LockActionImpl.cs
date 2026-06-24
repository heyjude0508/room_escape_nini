//using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LockActionImpl : MonoBehaviour, ILockAction
{
    //public GameEvent gameEventAimStart;
    //public GameEvent gameEventAimEnd;
    //public GameEvent gameEventInteract;

    //public DOTweenAnimation dtAnim;

    [SerializeField] LockPuzzle lockPuzzle;

    BagManagementImpl bag;

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

    public string GetDescription() => lockPuzzle.puzzleDesc;

    public void Interact()
    {
        Unlock();
    }

    public void Unlock()
    {

        if (bag == null)
        {
            Debug.LogError("Cannot find the bag.");
            return;
        }

        List<string> itemIdList = bag.GetItemIdList();
        if (itemIdList.Contains(lockPuzzle.solution))
        {
            if (GetComponent<Collider>() != null)
            {
                GetComponent<Collider>().enabled = false;
            }
            this.enabled = false;
            Debug.Log($"Unlock the lock {lockPuzzle.id} using the key {lockPuzzle.solution} successfully.");
        }
        else
        {
            Debug.Log("Need to find the key!");
            return;
        }
    }

}