using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Room Escape/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemId;
    public string displayName;
    public Sprite icon;
}
