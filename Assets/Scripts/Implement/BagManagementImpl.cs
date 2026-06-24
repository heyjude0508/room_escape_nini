//using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class BagManagementImpl : MonoBehaviour, IBagManagement
{
    public List<Item> itemList;
    public List<string> itemIdList;

    public static BagManagementImpl Instance { get; private set; }

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

    public void AddItem(Item item)
    {
        // 如果背包里还没有这个物品才放进去，防止重复捡起报错
        if (!itemList.Contains(item))
        {
            itemList.Add(item);
            Debug.Log($"Put item {item.id} into the bag successfully，total number of items: {itemList.Count}.");
        }
    }

    public void RemoveItem(Item item)
    {
        // 只有在背包里有这件物品时才移除
        if (itemList.Contains(item))
        {
            itemList.Remove(item);
            Debug.Log($"Get item {item.id} out of the bag successfully, total number of items: {itemList.Count}.");
        }
    }

    public List<string> GetItemIdList()
    {
        foreach (Item item in itemList)
        {
            itemIdList.Add(item.id);
        }
        return itemIdList;
    }

    public bool HasItem(Item item) => itemList.Contains(item);

}