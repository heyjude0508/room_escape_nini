//using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemKeyImpl : MonoBehaviour, IItemKey
{
    //public GameEvent gameEventAimStart;
    //public GameEvent gameEventAimEnd;
    //public GameEvent gameEventInteract;

    //public DOTweenAnimation dtAnim;

    [SerializeField] ItemKey itemKey;

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

    public string GetDescription() => itemKey.itemActionDesc;

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

    ItemKey CreateCopy()
    {
        ItemKey copy = new ItemKey();
        copy.id = itemKey.id;
        copy.itemName = itemKey.itemName;
        copy.itemSprite = itemKey.itemSprite;
        copy.itemActionDesc = itemKey.itemActionDesc;
        copy.itemUsageDesc = itemKey.itemUsageDesc;
        return copy;
    }

}
