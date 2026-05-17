using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyActionImpl: MonoBehaviour, IKeyAction
{
    [SerializeField] string keyID = "Default Key";
    string description = "Press E to pick up the key.";

    Renderer keyRenderer;

    IBagManagement bag;

    public GameEvent gameEventAimStart;
    public GameEvent gameEventAimEnd;
    public GameEvent gameEventInteract;

    public DOTweenAnimation dtAnim;

    // Start is called before the first frame update
    void Start()
    {
        //dtAnim.DOPlay();
        bag = BagManagementImpl.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    EventAimStart();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    EventAimEnd();
        //}
    }

    public void EventAimStart()
    {
        gameEventAimStart.Raise();
    }

    public void EventAimEnd()
    {
        gameEventAimEnd.Raise();
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
