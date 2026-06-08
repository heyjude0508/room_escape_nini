using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Slot 
{
    public Image itemSource;
    public string name;
    public int itemID;
    public Slot(Image _itemSource, string _name, int _itemID)
    {
        itemSource = _itemSource;
        name = _name;
        itemID = _itemID;
    }
}

public class InventoryController : MonoBehaviour
{
    // singleton
    public static InventoryController Instance 
    { 
        get { return instance; } 
        private set { } 
    }

    private static InventoryController instance;

    public List<Slot> slotList; // 7 ge

    public List<GameObject> slotsIcon;

    private void Awake()
    {
        
        for (int i = 0; i < this.transform.childCount; ++i)
        {
            slotsIcon.Add(this.transform.GetChild(i).gameObject);
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
    public void AddItemToMyList(Slot item)
    {
        
        // slot .add 
        UpdateSlotsAppearance();
    }

    public void RemoveItemFromList(Slot item)
    {
        if(slotList.Contains(item))
            slotList.Remove(item);

        UpdateSlotsAppearance();
    }

    public void UpdateSlotsAppearance()
    {

    }
}
