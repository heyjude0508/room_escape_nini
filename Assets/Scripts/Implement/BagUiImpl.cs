using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagUiImpl : MonoBehaviour, IBagUi
{
    public const int MaxItemSlots = BagManagementImpl.MaxItemSlots;
    public const int MaxFragmentSlots = BagManagementImpl.MaxFragmentSlots;
    const string ItemSlotPrefix = "ItemSlot_";
    const string FragmentSlotPrefix = "FragmentSlot_";
    const string ItemIconName = "ItemIcon";
    const string UsageTextName = "UsageText";
    const string DefaultUsageDesc = "Default Usage";

    [SerializeField] Transform itemSlotListRoot;
    [SerializeField] TMP_Text itemDescText;
    [SerializeField] KeyCode toggleKey = KeyCode.I;
    [SerializeField][Range(0.3f, 0.9f)] float itemIconFillRatio = 0.65f;
    [SerializeField][Range(0.3f, 0.9f)] float fragmentIconFillRatio = 0.65f;
    [SerializeField] TMP_FontAsset usageFont;
    [SerializeField] float usageFontSize = 24f;
    [SerializeField] Color usageFontColor = Color.white;

    public KeyCode ToggleKey => toggleKey;

    readonly List<Slot> slotList = new List<Slot>(MaxItemSlots);
    readonly List<Slot> fragmentSlotList = new List<Slot>(MaxFragmentSlots);
    BagManagementImpl bag;
    Canvas bagCanvas;
    bool isOpen;
    bool initialized;

    void Awake()
    {
        EnsureInitialized();
    }

    void OnEnable()
    {
        EnsureInitialized();
        isOpen = true;
        BindBagEvents();
        RefreshAllSlots();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnDisable()
    {
        isOpen = false;
        ClearDetailPanel();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Start()
    {
        BindBagEvents();
        RefreshAllSlots();
    }

    void OnDestroy()
    {
        if (bag != null)
        {
            bag.OnBagUpdated -= RefreshAllSlots;
        }
    }

    void EnsureInitialized()
    {
        if (initialized)
        {
            return;
        }

        AutoFindReferences();
        InitItemSlots();
        InitFragmentSlots();
        bagCanvas = GetComponentInParent<Canvas>();
        initialized = true;
    }

    void BindBagEvents()
    {
        if (bag != null)
        {
            return;
        }

        bag = BagManagementImpl.Instance;
        if (bag == null)
        {
            Debug.LogError("Cannot find BagManagementImpl.");
            return;
        }

        bag.OnBagUpdated += RefreshAllSlots;
    }

    public void AutoFindReferences()
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

    public void InitItemSlots()
    {
        InitSlotsByPrefix(ItemSlotPrefix, MaxItemSlots, slotList, "item", itemIconFillRatio);
    }

    public void InitFragmentSlots()
    {
        InitSlotsByPrefix(FragmentSlotPrefix, MaxFragmentSlots, fragmentSlotList, "fragment", fragmentIconFillRatio);
    }

    void InitSlotsByPrefix(
        string prefix,
        int maxSlots,
        List<Slot> targetList,
        string label,
        float iconFillRatio)
    {
        targetList.Clear();

        if (itemSlotListRoot == null)
        {
            Debug.LogError("ItemSlotList not found under BagPanel.");
            return;
        }

        List<Transform> slotTransforms = new List<Transform>();
        for (int i = 0; i < itemSlotListRoot.childCount; i++)
        {
            Transform child = itemSlotListRoot.GetChild(i);
            if (child.name.StartsWith(prefix, StringComparison.Ordinal))
            {
                slotTransforms.Add(child);
            }
        }

        slotTransforms.Sort((left, right) =>
            string.Compare(left.name, right.name, StringComparison.Ordinal));

        for (int i = 0; i < slotTransforms.Count && targetList.Count < maxSlots; i++)
        {
            Transform slotTransform = slotTransforms[i];
            Image frameImage = slotTransform.GetComponent<Image>();
            if (frameImage == null)
            {
                Debug.LogWarning($"Missing Image on {slotTransform.name}.");
                continue;
            }

            Image iconImage = GetOrCreateItemIconImage(slotTransform, iconFillRatio);
            Sprite emptySlotSprite = frameImage.sprite;
            targetList.Add(new Slot("", "", frameImage, iconImage, emptySlotSprite));
            ClearSlotVisual(targetList[targetList.Count - 1]);
        }

        if (targetList.Count != maxSlots)
        {
            Debug.LogWarning($"Bag UI expects {maxSlots} {label} slots, but initialized {targetList.Count}.");
        }
    }

    public void RefreshAllSlots()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            ClearSlotVisual(slotList[i]);
        }

        for (int i = 0; i < fragmentSlotList.Count; i++)
        {
            ClearSlotVisual(fragmentSlotList[i]);
        }

        ClearDetailPanel();

        if (bag == null)
        {
            return;
        }

        for (int i = 0; i < bag.itemList.Count && i < slotList.Count; i++)
        {
            SetSlotItem(slotList, i, bag.itemList[i]);
        }

        for (int i = 0; i < bag.fragmentList.Count && i < fragmentSlotList.Count; i++)
        {
            SetSlotItem(fragmentSlotList, i, bag.fragmentList[i]);
        }
    }

    public void SetSlotItem(int slotId, ItemBase item)
    {
        SetSlotItem(slotList, slotId, item);
    }

    void SetSlotItem(List<Slot> targetList, int slotId, ItemBase item)
    {
        if (slotId < 0 || slotId >= targetList.Count || item == null)
        {
            return;
        }

        Slot slot = targetList[slotId];
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

    public bool IsBagOpen()
    {
        return isOpen && gameObject.activeSelf;
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

        if (TrySelectFromSlotList(slotList, screenPoint, eventCamera, false))
        {
            return true;
        }

        return TrySelectFromSlotList(fragmentSlotList, screenPoint, eventCamera, true);
    }

    bool TrySelectFromSlotList(
        List<Slot> targetList,
        Vector2 screenPoint,
        Camera eventCamera,
        bool isFragment)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            RectTransform slotRect = targetList[i].frameImage.rectTransform;
            if (!RectTransformUtility.RectangleContainsScreenPoint(slotRect, screenPoint, eventCamera))
            {
                continue;
            }

            if (targetList[i].itemId.IsEmpty())
            {
                ClearDetailPanel();
            }
            else
            {
                SelectSlot(targetList, i, isFragment);
            }

            return true;
        }

        return false;
    }

    void SelectSlot(List<Slot> targetList, int index, bool isFragment)
    {
        if (index < 0 || index >= targetList.Count)
        {
            return;
        }

        Slot slot = targetList[index];
        if (slot.itemId.IsEmpty())
        {
            ClearDetailPanel();
            return;
        }

        ItemBase item = null;
        if (bag != null)
        {
            List<ItemBase> sourceList = isFragment ? bag.fragmentList : bag.itemList;
            item = sourceList.Find(existingItem => existingItem.id == slot.itemId);
        }

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

        if (usageFont != null)
        {
            itemDescText.font = usageFont;
        }

        itemDescText.fontSize = usageFontSize;
        itemDescText.color = usageFontColor;
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

    string GetUsageDescription(ItemBase item)
    {
        if (item == null || string.IsNullOrEmpty(item.itemUsageDesc))
        {
            return DefaultUsageDesc;
        }

        return item.itemUsageDesc;
    }

    Image GetOrCreateItemIconImage(Transform slotTransform, float iconFillRatio)
    {
        Transform existingIcon = slotTransform.Find(ItemIconName);
        if (existingIcon != null)
        {
            Image existingImage = existingIcon.GetComponent<Image>();
            if (existingImage != null)
            {
                ApplyItemIconLayout(existingIcon, slotTransform, iconFillRatio);
                return existingImage;
            }
        }

        GameObject iconObject = new GameObject(
            ItemIconName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));

        iconObject.transform.SetParent(slotTransform, false);
        ApplyItemIconLayout(iconObject.transform, slotTransform, iconFillRatio);

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
        usageText.font = usageFont;
        usageText.fontSize = usageFontSize;
        usageText.color = usageFontColor;
        usageText.text = "";
        return usageText;
    }

    void ApplyItemIconLayout(Transform iconTransform, Transform slotTransform, float iconFillRatio)
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
        float iconScreenSize = Mathf.Min(slotScreenWidth, slotScreenHeight) * iconFillRatio;

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
        SetBagVisible(!gameObject.activeSelf);
    }

    void SetBagVisible(bool visible)
    {
        // Visibility is driven by GameObject active state (BagPanel starts inactive).
        if (gameObject.activeSelf == visible)
        {
            isOpen = visible;
            return;
        }

        gameObject.SetActive(visible);
    }

}
