//using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyActionImpl : MonoBehaviour, IKeyAction
{
    //public GameEvent gameEventAimStart;
    //public GameEvent gameEventAimEnd;
    //public GameEvent gameEventInteract;

    //public DOTweenAnimation dtAnim;

    [SerializeField] KeyItem keyItem;

    Renderer keyRenderer;

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

    public string GetDescription() => keyItem.itemDesc;

    public void Interact()
    {
        PickKey();
    }

    public void PickKey()
    {

        if (bag != null)
        {
            bag.AddItem(keyItem);
        }
        else
        {
            Debug.LogError("Cannot find the bag.");
            return;
        }

        Destroy(gameObject);
    }

    // ±ä¸üµÀ¾ßµÄÑÕÉ«
    public void ChangeColor()
    {
        if (keyRenderer != null)
        {
            keyRenderer.material.color = new Color(Random.value, Random.value, Random.value);
        }
        else
        {
            keyRenderer = GetComponentInChildren<Renderer>();
            if (keyRenderer != null)
            {
                keyRenderer.material.color = new Color(Random.value, Random.value, Random.value);
            }
        }
    }

}