using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Npc
{
    public string id;
    public string npcName;
    public string npcAvatarName;
    public string npcDesc;
    public string npcAssgnDesc;

    public Npc(
        string id,
        string npcName,
        string npcAvatarName,
        string npcDesc,
        string npcAssgnDesc)
    {
        this.id = id;
        this.npcName = npcName;
        this.npcAvatarName = npcAvatarName;
        this.npcDesc = npcDesc;
        this.npcAssgnDesc = npcAssgnDesc;
    }

    public Npc() { }

}
