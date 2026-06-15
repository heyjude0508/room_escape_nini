using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagManagementImpl : MonoBehaviour, IBagManagement
{
    public static BagManagementImpl Instance { get; private set; }

    [SerializeField] Transform slotPanel;

    public List<GameObject> slotsIcon = new List<GameObject>();
    public List<Slot> slotList = new List<Slot>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSlots();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeSlots()
    {
        slotsIcon.Clear();
        slotList.Clear();

        if (slotPanel == null)
        {
            return;
        }

        for (int i = 0; i < slotPanel.childCount; i++)
        {
            slotsIcon.Add(slotPanel.GetChild(i).gameObject);
            slotList.Add(new Slot());
        }

        UpdateSlotsAppearance();
    }

    public void AddItem(string itemId)
    {
        if (HasItem(itemId))
        {
            return;
        }

        int emptyIndex = FindEmptySlotIndex();
        if (emptyIndex < 0)
        {
            Debug.LogWarning($"背包已满，无法放入物品: {itemId}");
            return;
        }

        slotList[emptyIndex].itemId = itemId;
        slotList[emptyIndex].displayName = itemId;
        Debug.Log($"背包系统成功放入物品: {itemId}，当前物品总数: {GetItemCount()}。");
        UpdateSlotsAppearance();
    }

    public void RemoveItem(string itemId)
    {
        int index = FindSlotIndexByItemId(itemId);
        if (index < 0)
        {
            return;
        }

        slotList[index].Clear();
        Debug.Log($"背包系统物品已移除: {itemId}。");
        UpdateSlotsAppearance();
    }

    public bool HasItem(string itemId) => FindSlotIndexByItemId(itemId) >= 0;

    public void UpdateSlotsAppearance()
    {
        for (int i = 0; i < slotsIcon.Count && i < slotList.Count; i++)
        {
            Image image = slotsIcon[i].GetComponent<Image>();
            if (image == null)
            {
                continue;
            }

            Slot slot = slotList[i];
            if (slot.IsEmpty)
            {
                Color color = image.color;
                image.color = new Color(color.r, color.g, color.b, 0.3f);
                continue;
            }

            if (slot.icon != null)
            {
                image.sprite = slot.icon;
            }

            Color filledColor = image.color;
            image.color = new Color(filledColor.r, filledColor.g, filledColor.b, 1f);
        }
    }

    int FindEmptySlotIndex()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].IsEmpty)
            {
                return i;
            }
        }

        return -1;
    }

    int FindSlotIndexByItemId(string itemId)
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].itemId == itemId)
            {
                return i;
            }
        }

        return -1;
    }

    int GetItemCount()
    {
        int count = 0;
        foreach (Slot slot in slotList)
        {
            if (!slot.IsEmpty)
            {
                count++;
            }
        }

        return count;
    }
}
