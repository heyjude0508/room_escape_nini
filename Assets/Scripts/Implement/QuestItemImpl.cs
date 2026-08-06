//using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItemImpl : MonoBehaviour, IQuestItem
{
    //public GameEvent gameEventAimStart;
    //public GameEvent gameEventAimEnd;
    //public GameEvent gameEventInteract;

    //public DOTweenAnimation dtAnim;

    [SerializeField] QuestItem questItem;

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

    public string GetDescription() => questItem.itemActionDesc;

    public void Interact()
    {
        PickItem();
    }

    public void PickItem()
    {

        if (bag != null)
        {
            bag.AddItem(questItem.CreateCopy());
        }
        else
        {
            Debug.LogError("Cannot find the bag.");
            return;
        }

        Destroy(gameObject);
    }

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
