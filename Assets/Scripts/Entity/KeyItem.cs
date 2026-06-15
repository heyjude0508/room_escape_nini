using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[System.Serializable]
public class KeyItem: Item
{
    public KeyItem(): base()
    {
        id = "DefaultKey";
        itemName = "DefaultKey";
        itemDesc = "Press E to pick up the key.";
        itemSprite = null;
    }

}
