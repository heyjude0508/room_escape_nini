using UnityEngine;

public interface IBagUi
{
    public void AddItem(Item item);

    public int GetMinEmptySlotId();

    public bool IsBagOpen();

    public bool TrySelectItemAtScreenPoint(Vector2 screenPoint);

}
