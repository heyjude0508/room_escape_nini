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

    [SerializeField] string keyID = "Default Key";
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
            bag.AddItem(keyID);
        }
        else
        {
            Debug.LogError("找不到背包。");
            return;
        }

        Destroy(gameObject);
    }

    // 变更道具的颜色
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
