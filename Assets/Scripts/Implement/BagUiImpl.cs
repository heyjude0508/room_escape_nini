using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagUiImpl : MonoBehaviour, IBagUi
{
    public const int MaxItemSlots = BagManagementImpl.MaxItemSlots;
    const string ItemSlotPrefix = "ItemSlot_";
    const string ItemIconName = "ItemIcon";
    const string UsageTextName = "UsageText";
    const string DefaultUsageDesc = "Default Usage";

    [SerializeField] Transform itemSlotListRoot;
    [SerializeField] TMP_Text itemDescText;
    [SerializeField] KeyCode toggleKey = KeyCode.I;
    [SerializeField][Range(0.3f, 0.9f)] float itemIconFillRatio = 0.65f;
    [SerializeField] float defaultUsageFontSize = 24f;
    [SerializeField] TMP_FontAsset defaultUsageFont;

    readonly List<Slot> slotList = new List<Slot>(MaxItemSlots);
    BagManagementImpl bag;
    CanvasGroup canvasGroup;
    Canvas bagCanvas;
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

        bagCanvas = GetComponentInParent<Canvas>();
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

        Transform descPanel = transform.Find("DescPanel");
        if (descPanel != null && itemDescText == null)
        {
            itemDescText = GetOrCreateUsageText(descPanel);
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
            Image frameImage = slotTransform.GetComponent<Image>();
            if (frameImage == null)
            {
                Debug.LogWarning($"Missing Image on {slotTransform.name}.");
                continue;
            }

            Image iconImage = GetOrCreateItemIconImage(slotTransform);
            Sprite emptySlotSprite = frameImage.sprite;
            slotList.Add(new Slot("", "", frameImage, iconImage, emptySlotSprite));
            ClearSlotVisual(slotList[slotList.Count - 1]);
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
        slot.itemSprite = item.itemSprite;

        slot.frameImage.sprite = slot.emptySlotSprite;
        slot.frameImage.enabled = slot.emptySlotSprite != null;

        if (item.itemSprite != null)
        {
            slot.iconImage.sprite = item.itemSprite;
            slot.iconImage.enabled = true;
            return;
        }

        slot.iconImage.sprite = null;
        slot.iconImage.enabled = false;
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

    public bool IsBagOpen()
    {
        return isOpen;
    }

    public bool TrySelectItemAtScreenPoint(Vector2 screenPoint)
    {
        if (!isOpen)
        {
            return false;
        }

        Camera eventCamera = null;
        if (bagCanvas != null && bagCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            eventCamera = bagCanvas.worldCamera;
        }

        for (int i = 0; i < slotList.Count; i++)
        {
            RectTransform slotRect = slotList[i].frameImage.rectTransform;
            if (!RectTransformUtility.RectangleContainsScreenPoint(slotRect, screenPoint, eventCamera))
            {
                continue;
            }

            if (slotList[i].itemId.IsEmpty())
            {
                ClearDetailPanel();
            }
            else
            {
                SelectSlot(i);
            }

            return true;
        }

        return false;
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

        Item item = bag != null
            ? bag.itemList.Find(existingItem => existingItem.id == slot.itemId)
            : null;

        if (itemDescText != null)
        {
            ApplyUsageDescStyle();
            itemDescText.text = GetUsageDescription(item);
        }
    }

    void ApplyUsageDescStyle()
    {
        if (itemDescText == null)
        {
            return;
        }

        if (defaultUsageFont != null)
        {
            itemDescText.font = defaultUsageFont;
        }

        itemDescText.fontSize = defaultUsageFontSize;
    }

    void ClearSlotVisual(Slot slot)
    {
        slot.itemId = "";
        slot.itemName = "";
        slot.itemSprite = null;
        slot.frameImage.sprite = slot.emptySlotSprite;
        slot.frameImage.enabled = slot.emptySlotSprite != null;
        slot.iconImage.sprite = null;
        slot.iconImage.enabled = false;
    }

    void ClearDetailPanel()
    {
        if (itemDescText != null)
        {
            itemDescText.text = "";
        }
    }

    string GetUsageDescription(Item item)
    {
        if (item == null || string.IsNullOrEmpty(item.itemUsageDesc))
        {
            return DefaultUsageDesc;
        }

        return item.itemUsageDesc;
    }

    Image GetOrCreateItemIconImage(Transform slotTransform)
    {
        Transform existingIcon = slotTransform.Find(ItemIconName);
        if (existingIcon != null)
        {
            Image existingImage = existingIcon.GetComponent<Image>();
            if (existingImage != null)
            {
                ApplyItemIconLayout(existingIcon, slotTransform);
                return existingImage;
            }
        }

        GameObject iconObject = new GameObject(
            ItemIconName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));

        iconObject.transform.SetParent(slotTransform, false);
        ApplyItemIconLayout(iconObject.transform, slotTransform);

        Image iconImage = iconObject.GetComponent<Image>();
        iconImage.raycastTarget = false;
        iconImage.preserveAspect = true;
        iconImage.enabled = false;
        return iconImage;
    }

    TMP_Text GetOrCreateUsageText(Transform descPanel)
    {
        Transform existingText = descPanel.Find(UsageTextName);
        if (existingText != null)
        {
            TMP_Text existing = existingText.GetComponent<TMP_Text>();
            if (existing != null)
            {
                ApplyUsageTextLayout(existingText);
                return existing;
            }
        }

        GameObject textObject = new GameObject(UsageTextName, typeof(RectTransform));
        textObject.transform.SetParent(descPanel, false);
        ApplyUsageTextLayout(textObject.transform);

        TextMeshProUGUI usageText = textObject.AddComponent<TextMeshProUGUI>();
        usageText.raycastTarget = false;
        usageText.alignment = TextAlignmentOptions.TopLeft;
        usageText.font = defaultUsageFont;
        usageText.fontSize = defaultUsageFontSize;
        usageText.color = Color.white;
        usageText.text = "";
        return usageText;
    }

    void ApplyItemIconLayout(Transform iconTransform, Transform slotTransform)
    {
        RectTransform rectTransform = iconTransform as RectTransform;
        RectTransform slotRectTransform = slotTransform as RectTransform;
        if (rectTransform == null || slotRectTransform == null)
        {
            return;
        }

        Vector3 slotScale = slotTransform.localScale;
        float scaleX = Mathf.Approximately(slotScale.x, 0f) ? 1f : slotScale.x;
        float scaleY = Mathf.Approximately(slotScale.y, 0f) ? 1f : slotScale.y;
        float slotScreenWidth = slotRectTransform.sizeDelta.x * scaleX;
        float slotScreenHeight = slotRectTransform.sizeDelta.y * scaleY;
        float iconScreenSize = Mathf.Min(slotScreenWidth, slotScreenHeight) * itemIconFillRatio;

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(iconScreenSize / scaleX, iconScreenSize / scaleY);
        rectTransform.localScale = Vector3.one;
    }

    void ApplyUsageTextLayout(Transform textTransform)
    {
        RectTransform rectTransform = textTransform as RectTransform;
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(12f, 12f);
        rectTransform.offsetMax = new Vector2(-12f, -12f);
        rectTransform.localScale = Vector3.one;
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

        if (visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            ClearDetailPanel();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

}
