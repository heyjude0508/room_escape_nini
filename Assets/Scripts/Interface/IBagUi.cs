using UnityEngine;

public interface IBagUi
{
    public void AddItem(ItemBase item);

    public int GetMinEmptySlotId();

    public bool IsBagOpen();

    public bool TrySelectItemAtScreenPoint(Vector2 screenPoint);

}
