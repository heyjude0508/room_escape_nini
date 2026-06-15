using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagManagementImpl : MonoBehaviour, IBagManagement
{
    const string IconChildName = "Icon";

    public static BagManagementImpl Instance { get; private set; }

    [SerializeField] Transform slotPanel;
    [SerializeField] Color emptySlotColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] Color filledSlotColor = new Color(1f, 1f, 1f, 1f);

    public List<GameObject> slotsIcon = new List<GameObject>();
    public List<Slot> slotList = new List<Slot>();

    readonly Dictionary<string, Image> slotIconImages = new Dictionary<string, Image>();

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
        slotIconImages.Clear();

        if (slotPanel == null)
        {
            return;
        }

        for (int i = 0; i < slotPanel.childCount; i++)
        {
            GameObject slotObject = slotPanel.GetChild(i).gameObject;
            slotsIcon.Add(slotObject);
            slotList.Add(new Slot());
            GetOrCreateIconImage(slotObject);
        }

        UpdateSlotsAppearance();
    }

    public void AddItem(string itemId, Sprite icon = null, string displayName = null)
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

        Slot slot = slotList[emptyIndex];
        slot.itemId = itemId;
        slot.displayName = string.IsNullOrEmpty(displayName) ? itemId : displayName;
        slot.icon = icon;
        Debug.Log($"背包系统成功放入物品: {itemId}，当前物品总数: {GetItemCount()}。");
        UpdateSlotsAppearance();
    }

    public void AddItem(ItemData itemData)
    {
        if (itemData == null)
        {
            return;
        }

        AddItem(itemData.itemId, itemData.icon, itemData.displayName);
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
            GameObject slotObject = slotsIcon[i];
            Image background = slotObject.GetComponent<Image>();
            Image iconImage = GetOrCreateIconImage(slotObject);
            Slot slot = slotList[i];

            if (background != null)
            {
                background.color = slot.IsEmpty ? emptySlotColor : filledSlotColor;
            }

            if (slot.IsEmpty || slot.icon == null)
            {
                iconImage.enabled = false;
                iconImage.sprite = null;
                continue;
            }

            iconImage.sprite = slot.icon;
            iconImage.color = Color.white;
            iconImage.enabled = true;
        }
    }

    Image GetOrCreateIconImage(GameObject slotObject)
    {
        if (slotIconImages.TryGetValue(slotObject.name, out Image cachedIcon))
        {
            return cachedIcon;
        }

        Transform iconTransform = slotObject.transform.Find(IconChildName);
        if (iconTransform == null)
        {
            GameObject iconObject = new GameObject(IconChildName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconTransform = iconObject.transform;
            iconTransform.SetParent(slotObject.transform, false);

            RectTransform rect = iconTransform as RectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(10f, 10f);
            rect.offsetMax = new Vector2(-10f, -10f);
            rect.localScale = Vector3.one;
        }

        Image iconImage = iconTransform.GetComponent<Image>();
        iconImage.raycastTarget = false;
        iconImage.preserveAspect = true;
        iconImage.enabled = false;
        slotIconImages[slotObject.name] = iconImage;
        return iconImage;
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
