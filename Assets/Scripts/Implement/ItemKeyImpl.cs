using UnityEngine;

public class ItemKeyImpl : MonoBehaviour, IItemKey
{
    [SerializeField] ItemKey itemKey;

    BagManagementImpl bag;

    void Start()
    {
        bag = BagManagementImpl.Instance;
    }

    public void EventAimStart()
    {
    }

    public void EventAimEnd()
    {
    }

    public void EventInteract()
    {
    }

    public string GetDescription() => itemKey.itemActionDesc;

    public void Interact()
    {
        PickItem();
    }

    public void PickItem()
    {
        if (bag == null)
        {
            Debug.LogError("Cannot find the bag.");
            return;
        }

        ItemKey copy = CreateCopy();
        if (BagManagementImpl.IsFragmentId(copy.id))
        {
            bag.AddFragment(copy);
        }
        else
        {
            bag.AddItem(copy);
        }

        HouseChildSequencer sequencer = FindObjectOfType<HouseChildSequencer>();
        if (sequencer != null)
        {
            sequencer.NotifyItemPicked(copy.id);
        }

        Destroy(gameObject);
    }

    ItemKey CreateCopy()
    {
        ItemKey copy = new ItemKey();
        copy.id = itemKey.id;
        copy.itemName = itemKey.itemName;
        copy.itemSprite = itemKey.itemSprite;
        copy.itemActionDesc = itemKey.itemActionDesc;
        copy.itemUsageDesc = itemKey.itemUsageDesc;
        return copy;
    }

}
