using UnityEngine;

public interface IBagUi
{
    public void AddItem(Item item);

    public int GetMinEmptySlotId();

}
