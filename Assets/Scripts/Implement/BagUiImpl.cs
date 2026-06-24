using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagUiImpl : MonoBehaviour, IBagUi
{
    [SerializeField] Transform slotListRoot;
    [SerializeField] Image itemDetailImage;
    [SerializeField] TMP_Text itemDescText;

    readonly List<Slot> slotList = new List<Slot>();
    BagManagementImpl bag;

    void Awake()
    {
        if (slotListRoot == null)
        {
            slotListRoot = transform.Find("SlotList");
        }

        InitSlots();
    }

    void Start()
    {
        bag = BagManagementImpl.Instance;
        if (bag == null)
        {
            Debug.LogError("Cannot find BagManagementImpl.");
            return;
        }

        bag.OnBagUpdated += RefreshAllSlots;
        RefreshAllSlots();
    }

    void OnDestroy()
    {
        if (bag != null)
        {
            bag.OnBagUpdated -= RefreshAllSlots;
        }
    }

    void InitSlots()
    {
        slotList.Clear();

        if (slotListRoot == null)
        {
            Debug.LogError("SlotList not found under BagPanel.");
            return;
        }

        for (int i = 0; i < slotListRoot.childCount; i++)
        {
            Transform slotTransform = slotListRoot.GetChild(i);
            Image iconImage = slotTransform.GetComponent<Image>();
            if (iconImage == null)
            {
                continue;
            }

            iconImage.enabled = false;
            slotList.Add(new Slot("", "", iconImage));

            int slotIndex = i;
            Button slotButton = slotTransform.GetComponent<Button>();
            if (slotButton == null)
            {
                slotButton = slotTransform.gameObject.AddComponent<Button>();
            }

            slotButton.onClick.AddListener(() => SelectSlot(slotIndex));
        }
    }

    void RefreshAllSlots()
    {
        ClearAllSlots();

        if (bag == null)
        {
            return;
        }

        for (int i = 0; i < bag.itemList.Count && i < slotList.Count; i++)
        {
            SetSlotItem(i, bag.itemList[i]);
        }
    }

    public void AddItem(Item item)
    {
        int slotId = GetMinEmptySlotId();
        if (slotId == -1)
        {
            Debug.LogWarning("The bag is full!");
            return;
        }

        SetSlotItem(slotId, item);
    }

    void SetSlotItem(int slotId, Item item)
    {
        slotList[slotId].itemId = item.id;
        slotList[slotId].itemName = item.itemName;
        slotList[slotId].iconImage.sprite = item.itemSprite;
        slotList[slotId].iconImage.enabled = item.itemSprite != null;
    }

    public int GetMinEmptySlotId()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (string.IsNullOrEmpty(slotList[i].itemId))
            {
                return i;
            }
        }

        return -1;
    }

    public void RemoveItemFromSlot(Item item)
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].itemId != item.id)
            {
                continue;
            }

            slotList[i].itemId = "";
            slotList[i].itemName = "";
            slotList[i].iconImage.sprite = null;
            slotList[i].iconImage.enabled = false;
            ClearDetailPanel();
            return;
        }

        Debug.LogWarning("Item not found in bag!");
    }

    void SelectSlot(int index)
    {
        if (index < 0 || index >= slotList.Count)
        {
            return;
        }

        Slot slot = slotList[index];
        if (string.IsNullOrEmpty(slot.itemId))
        {
            ClearDetailPanel();
            return;
        }

        if (itemDetailImage != null)
        {
            itemDetailImage.sprite = slot.iconImage.sprite;
            itemDetailImage.enabled = slot.iconImage.sprite != null;
        }

        if (itemDescText != null && bag != null)
        {
            Item item = bag.itemList.Find(existingItem => existingItem.id == slot.itemId);
            itemDescText.text = item != null ? item.itemDesc : slot.itemName;
        }
    }

    void ClearAllSlots()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].itemId = "";
            slotList[i].itemName = "";
            slotList[i].iconImage.sprite = null;
            slotList[i].iconImage.enabled = false;
        }

        ClearDetailPanel();
    }

    void ClearDetailPanel()
    {
        if (itemDetailImage != null)
        {
            itemDetailImage.sprite = null;
            itemDetailImage.enabled = false;
        }

        if (itemDescText != null)
        {
            itemDescText.text = "";
        }
    }
}
