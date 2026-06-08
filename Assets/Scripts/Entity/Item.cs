using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IPlayerAction
{
    public int id;
    //public string itemName;
        public string description;
    public void EventAimEnd()
    {
       
    }

    public void EventAimStart()
    {
        
    }

    public void EventInteract()
    {
        
    }

    public string GetDescription()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        //Slot slot = new Slot(,"Key1", 1);
        //InventoryController.Instance.AddItemToMyList(slot);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
