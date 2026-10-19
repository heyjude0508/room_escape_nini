using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class BagManagementImpl : MonoBehaviour, IBagManager
{
    public const int MaxItemSlots = 8;
    public const int MaxFragmentSlots = 6;

    const string TipPanelName = "TipPanel";
    const string InteractTipName = "InteractTip";
    const string FullBagTipName = "FullBagTip";
    const string FullBagMessage = "Bag is full!";
    const float FullBagTipSeconds = 2f;

    public List<ItemBase> itemList = new List<ItemBase>();
    public List<ItemBase> fragmentList = new List<ItemBase>();
    public List<string> itemIdList = new List<string>();

    public static BagManagementImpl Instance { get; private set; }

    public event Action OnBagUpdated;

    GameObject tipPanel;
    GameObject interactTip;
    GameObject fullBagTip;
    TMP_Text fullBagTipText;
    Coroutine fullBagTipHideRoutine;
    bool tipPanelWasActive;
    bool interactTipWasActive;

    public static bool IsFragmentId(string itemId)
    {
        return !string.IsNullOrEmpty(itemId)
            && itemId.StartsWith("F", StringComparison.Ordinal);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddItem(ItemBase item)
    {
        if (item == null)
        {
            return false;
        }

        if (IsFragmentId(item.id))
        {
            return AddFragment(item);
        }

        if (HasItem(item.id))
        {
            return false;
        }

        if (itemList.Count >= MaxItemSlots)
        {
            Debug.LogWarning("The bag is full!");
            ShowFullBagTip();
            return false;
        }

        itemList.Add(item);
        Debug.Log($"Put item {item.id} into the bag successfully, total number of items: {itemList.Count}.");

        OnBagUpdated?.Invoke();
        return true;
    }

    public bool AddFragment(ItemBase item)
    {
        if (item == null)
        {
            return false;
        }

        if (HasItem(item.id))
        {
            return false;
        }

        if (fragmentList.Count >= MaxFragmentSlots)
        {
            Debug.LogWarning("The fragment slots are full!");
            return false;
        }

        fragmentList.Add(item);
        Debug.Log($"Put fragment {item.id} into the bag successfully, total fragments: {fragmentList.Count}.");

        OnBagUpdated?.Invoke();
        return true;
    }

    public void RemoveItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return;
        }

        ItemBase existingItem = itemList.FirstOrDefault(existing => existing.id == itemId);
        if (existingItem != null)
        {
            itemList.Remove(existingItem);
            Debug.Log($"Get item {itemId} out of the bag successfully, total number of items: {itemList.Count}.");
            OnBagUpdated?.Invoke();
            return;
        }

        ItemBase existingFragment = fragmentList.FirstOrDefault(existing => existing.id == itemId);
        if (existingFragment == null)
        {
            return;
        }

        fragmentList.Remove(existingFragment);
        Debug.Log($"Get fragment {itemId} out of the bag successfully, total fragments: {fragmentList.Count}.");
        OnBagUpdated?.Invoke();
    }

    public bool HasItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            return false;
        }

        return itemList.Any(existingItem => existingItem.id == itemId)
            || fragmentList.Any(existingItem => existingItem.id == itemId);
    }

    public List<string> GetItemIdList()
    {
        itemIdList.Clear();
        foreach (ItemBase item in itemList)
        {
            itemIdList.Add(item.id);
        }

        foreach (ItemBase fragment in fragmentList)
        {
            itemIdList.Add(fragment.id);
        }

        return itemIdList;
    }

    void ShowFullBagTip()
    {
        CacheFullBagTipRefs();
        if (fullBagTip == null)
        {
            return;
        }

        tipPanelWasActive = tipPanel != null && tipPanel.activeSelf;
        interactTipWasActive = interactTip != null && interactTip.activeSelf;

        if (tipPanel != null)
        {
            tipPanel.SetActive(true);
        }

        if (interactTip != null)
        {
            interactTip.SetActive(false);
        }

        if (fullBagTipText != null)
        {
            fullBagTipText.text = FullBagMessage;
        }

        fullBagTip.SetActive(true);

        if (fullBagTipHideRoutine != null)
        {
            StopCoroutine(fullBagTipHideRoutine);
        }

        fullBagTipHideRoutine = StartCoroutine(HideFullBagTipAfterDelay());
    }

    IEnumerator HideFullBagTipAfterDelay()
    {
        yield return new WaitForSeconds(FullBagTipSeconds);
        HideFullBagTip();
    }

    void HideFullBagTip()
    {
        fullBagTipHideRoutine = null;

        if (fullBagTip != null)
        {
            fullBagTip.SetActive(false);
        }

        if (interactTip != null)
        {
            interactTip.SetActive(interactTipWasActive);
        }

        if (tipPanel != null)
        {
            tipPanel.SetActive(tipPanelWasActive);
        }
    }

    void CacheFullBagTipRefs()
    {
        if (fullBagTip != null)
        {
            return;
        }

        Transform tipPanelTransform = FindNamedUnderCanvases(TipPanelName);
        if (tipPanelTransform == null)
        {
            return;
        }

        tipPanel = tipPanelTransform.gameObject;

        Transform interactTipTransform = tipPanelTransform.Find(InteractTipName);
        if (interactTipTransform != null)
        {
            interactTip = interactTipTransform.gameObject;
        }

        Transform fullBagTipTransform = tipPanelTransform.Find(FullBagTipName);
        if (fullBagTipTransform == null)
        {
            return;
        }

        fullBagTip = fullBagTipTransform.gameObject;
        fullBagTipText = fullBagTip.GetComponentInChildren<TMP_Text>(true);
    }

    static Transform FindNamedUnderCanvases(string objectName)
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>(true);
        for (int i = 0; i < canvases.Length; i++)
        {
            Transform found = FindChildRecursive(canvases[i].transform, objectName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    static Transform FindChildRecursive(Transform parent, string childName)
    {
        if (parent == null)
        {
            return null;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == childName)
            {
                return child;
            }

            Transform nested = FindChildRecursive(child, childName);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }
}
