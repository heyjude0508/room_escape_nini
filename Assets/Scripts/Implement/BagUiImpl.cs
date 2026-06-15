using NaughtyAttributes.Test;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IBagUiImpl : MonoBehaviour, IBagUi
{
    public List<Slot> slotList;

    private void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            slotList.Add(transform.GetChild(i).GetComponent<Slot>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSlotImage(Item item)
    {
        int slotId = GetMinEmptySlotId();
        if (slotId == -1)
        {
            Debug.LogWarning("The bag is full!");
            return;
        }
        slotList[slotId].iconImage.sprite = item.itemSprite;
    }

    public int GetMinEmptySlotId()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (slotList[i] == null)
            {
                return i;
            }
        }

        // 循环走完了都没触发上面的 return，说明背包已经全满
        return -1;
    }

    public void RemoveItemFromSlot(Item item)
    { 
        
    }

}
