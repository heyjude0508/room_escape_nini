//using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagManagementImpl: MonoBehaviour, IBagManagement
{
    public static BagManagementImpl Instance { get; private set; }
    public List<Slot> slotList;
    public List<GameObject> slotsIcon;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void Awake()
    {
        // 创建背包单例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 背包跨关卡不销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(string itemId)
    {
        // 如果背包里还没有这个物品才放进去，防止重复捡起报错
        if (!bagItems.Contains(itemId))
        {
            bagItems.Add(itemId);
            Debug.Log($"背包系统成功放入物品: {itemId}，当前物品总数: {bagItems.Count}。");
        }
    }

    public void RemoveItem(string itemId)
    {
        // 只有在背包里有这件物品时才移除
        if (bagItems.Contains(itemId))
        {
            bagItems.Remove(itemId);
            Debug.Log($"背包系统物品已移除: {itemId}。");
        }
    }

    public bool HasItem(string itemId) => bagItems.Contains(itemId);

}
