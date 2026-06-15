//using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyActionImpl: MonoBehaviour, IKeyAction
{
    //public GameEvent gameEventAimStart;
    //public GameEvent gameEventAimEnd;
    //public GameEvent gameEventInteract;

    //public DOTweenAnimation dtAnim;

    IBagManagement bag;

    [SerializeField] ItemData itemData;
    [SerializeField] string keyID = "DefaultKey";
    string description = "Press E to pick up the key.";

    Renderer keyRenderer;

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

    public string GetDescription() => description;

    public void Interact()
    {
        PickKey();
    }

    public void PickKey()
    {

        if (bag != null) 
        {
            if (itemData != null)
            {
                bag.AddItem(itemData.itemId, itemData.icon, itemData.displayName);
            }
            else
            {
                bag.AddItem(keyID);
            }
        }
        else
        {
            Debug.LogError("???????????");
            return;
        }

        Destroy(gameObject);
    }

    // ???????????
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
            //gameEventInteract.Raise();
        }
    }

}
