using UnityEngine;

public interface IBagUi
{
    public void AutoFindReferences();

    public void InitItemSlots(); 
    
    public void RefreshAllSlots();

    public void SetSlotItem(int slotId, ItemBase item);

    public bool IsBagOpen();

    public bool TrySelectItemAtScreenPoint(Vector2 screenPoint);

    public void ToggleBag();

}
