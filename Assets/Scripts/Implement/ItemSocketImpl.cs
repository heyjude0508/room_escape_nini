//using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSocketImpl : MonoBehaviour, IItemSocket
{
    //public GameEvent gameEventAimStart;
    //public GameEvent gameEventAimEnd;
    //public GameEvent gameEventInteract;

    //public DOTweenAnimation dtAnim;

    [SerializeField] ItemSocket itemSocket;

    BagManagementImpl bag;

    // Start is called before the first frame update
    void Start()
    {
        bag = BagManagementImpl.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EventAimStart()
    {
        //if (gameEventAimStart != null)
        //{
        //    gameEventAimStart.Raise();
        //}

        //if (dtAnim != null)
        //{
        //    dtAnim.DOPlay();
        //}
    }

    public void EventAimEnd()
    {
        //gameEventAimEnd.Raise();
    }

    public void EventInteract()
    {
        //gameEventInteract.Raise();
    }

    public string GetDescription() => itemSocket.itemActionDesc;

    public void Interact()
    {
        PickItem();
    }

    public void PickItem()
    {
        if (bag != null)
        {
            bag.AddItem(CreateCopy());
        }
        else
        {
            Debug.LogError("Cannot find the bag.");
            return;
        }

        Destroy(gameObject);
    }

    ItemSocket CreateCopy()
    {
        ItemSocket copy = new ItemSocket();
        copy.id = itemSocket.id;
        copy.itemName = itemSocket.itemName;
        copy.itemSprite = itemSocket.itemSprite;
        copy.itemActionDesc = itemSocket.itemActionDesc;
        copy.itemUsageDesc = itemSocket.itemUsageDesc;
        return copy;
    }

}
