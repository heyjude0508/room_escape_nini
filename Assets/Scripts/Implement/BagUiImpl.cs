using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagUiImpl : MonoBehaviour, IBagUi
{
    public const int MaxItemSlots = BagManagementImpl.MaxItemSlots;
    const string ItemSlotPrefix = "ItemSlot_";

    [SerializeField] Transform itemSlotListRoot;
    [SerializeField] Image itemDetailImage;
    [SerializeField] TMP_Text itemDescText;
    [SerializeField] KeyCode toggleKey = KeyCode.I;

    readonly List<Slot> slotList = new List<Slot>(MaxItemSlots);
    BagManagementImpl bag;
    CanvasGroup canvasGroup;
    Sprite emptyDetailSprite;
    bool isOpen;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        AutoFindReferences();
        InitItemSlots();
        SetBagVisible(false);
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

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleBag();
        }
    }

    void OnDestroy()
    {
        if (bag != null)
        {
            bag.OnBagUpdated -= RefreshAllSlots;
        }
    }

    void AutoFindReferences()
    {
        if (itemSlotListRoot == null)
        {
            itemSlotListRoot = transform.Find("ItemSlotList");
        }

        if (itemDetailImage == null)
        {
            Transform prefabPanel = transform.Find("PrefabPanel");
            if (prefabPanel != null)
            {
                itemDetailImage = prefabPanel.GetComponent<Image>();
            }
        }

        if (itemDetailImage != null)
        {
            emptyDetailSprite = itemDetailImage.sprite;
        }

        if (itemDescText == null)
        {
            Transform descPanel = transform.Find("DescPanel");
            if (descPanel != null)
            {
                itemDescText = descPanel.GetComponentInChildren<TMP_Text>(true);
            }
        }
    }

    void InitItemSlots()
    {
        slotList.Clear();

        if (itemSlotListRoot == null)
        {
            Debug.LogError("ItemSlotList not found under BagPanel.");
            return;
        }

        List<Transform> itemSlotTransforms = new List<Transform>();
        for (int i = 0; i < itemSlotListRoot.childCount; i++)
        {
            Transform child = itemSlotListRoot.GetChild(i);
            if (child.name.StartsWith(ItemSlotPrefix, StringComparison.Ordinal))
            {
                itemSlotTransforms.Add(child);
            }
        }

        itemSlotTransforms.Sort((left, right) =>
            string.Compare(left.name, right.name, StringComparison.Ordinal));

        for (int i = 0; i < itemSlotTransforms.Count && slotList.Count < MaxItemSlots; i++)
        {
            Transform slotTransform = itemSlotTransforms[i];
            Image iconImage = slotTransform.GetComponent<Image>();
            if (iconImage == null)
            {
                Debug.LogWarning($"Missing Image on {slotTransform.name}.");
                continue;
            }

            Sprite emptySlotSprite = iconImage.sprite;
            slotList.Add(new Slot("", "", iconImage, emptySlotSprite));
            ClearSlotVisual(slotList[slotList.Count - 1]);

            int slotIndex = slotList.Count - 1;
            Button slotButton = slotTransform.GetComponent<Button>();
            if (slotButton == null)
            {
                slotButton = slotTransform.gameObject.AddComponent<Button>();
            }

            slotButton.onClick.AddListener(() => SelectSlot(slotIndex));
        }

        if (slotList.Count != MaxItemSlots)
        {
            Debug.LogWarning($"Bag UI expects {MaxItemSlots} item slots, but initialized {slotList.Count}.");
        }
    }

    void RefreshAllSlots()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            ClearSlotVisual(slotList[i]);
        }

        ClearDetailPanel();

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
        Slot slot = slotList[slotId];
        slot.itemId = item.id;
        slot.itemName = item.itemName;
        slot.iconImage.sprite = item.itemSprite != null ? item.itemSprite : slot.emptySlotSprite;
        slot.iconImage.enabled = true;
    }

    public int GetMinEmptySlotId()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i].itemId.IsEmpty())
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

            ClearSlotVisual(slotList[i]);
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
        if (slot.itemId.IsEmpty())
        {
            ClearDetailPanel();
            return;
        }

        if (itemDetailImage != null)
        {
            itemDetailImage.sprite = slot.iconImage.sprite != null
                ? slot.iconImage.sprite
                : emptyDetailSprite;
            itemDetailImage.enabled = true;
        }

        if (itemDescText != null && bag != null)
        {
            Item item = bag.itemList.Find(existingItem => existingItem.id == slot.itemId);
            itemDescText.text = item != null ? item.itemDesc : slot.itemName;
        }
    }

    void ClearSlotVisual(Slot slot)
    {
        slot.itemId = "";
        slot.itemName = "";
        slot.iconImage.sprite = slot.emptySlotSprite;
        slot.iconImage.enabled = slot.emptySlotSprite != null;
    }

    void ClearDetailPanel()
    {
        if (itemDetailImage != null)
        {
            itemDetailImage.sprite = emptyDetailSprite;
            itemDetailImage.enabled = emptyDetailSprite != null;
        }

        if (itemDescText != null)
        {
            itemDescText.text = "";
        }
    }

    public void ToggleBag()
    {
        SetBagVisible(!isOpen);
    }

    void SetBagVisible(bool visible)
    {
        isOpen = visible;
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;

        if (!visible)
        {
            ClearDetailPanel();
        }
    }

}
