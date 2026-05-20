using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockActionImpl: MonoBehaviour, ILockAction
{
    public GameEvent gameEventAimStart;
    public GameEvent gameEventAimEnd;
    public GameEvent gameEventInteract;

    public DOTweenAnimation dtAnim;

    IBagManagement bag;

    [SerializeField] string lockID = "DefaultLock";
    [SerializeField] string requiredKeyID = "DefaultKey";
    string description = "Press E to unlock.";

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

    public void EventInteract()
    {
        gameEventInteract.Raise();
    }

    public string GetDescription() => description;

    public void Interact()
    {
        Unlock();
    }

    public void Unlock()
    {

        if (bag == null) 
        {
            Debug.LogError("找不到背包。");
            return;
        }

        if (bag.HasItem(requiredKeyID))
        {
            bag.RemoveItem(requiredKeyID);

            if (GetComponent<Collider>() != null)
            {
                GetComponent<Collider>().enabled = false;
            }

            this.enabled = false;
            Debug.Log($"成功使用钥匙 [{requiredKeyID}] 打开了锁 [{lockID}]！");
        }
        else
        {
            Debug.Log($"需要找到钥匙！");
            return;
        }
    }

}
