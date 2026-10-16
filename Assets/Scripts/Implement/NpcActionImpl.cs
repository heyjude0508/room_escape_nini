using UnityEngine;

public class NpcActionImpl : MonoBehaviour, INpcAction
{
    [SerializeField] Npc npc;

    public void EventAimStart()
    {
    }

    public void EventAimEnd()
    {
    }

    public void EventInteract()
    {
    }

    public string GetDescription()
    {
        if (npc == null)
        {
            return string.Empty;
        }

        return npc.npcDesc;
    }

    public void Interact()
    {
        if (npc == null)
        {
            Debug.LogError("Npc data is missing.", this);
            return;
        }

        HouseChildSequencer sequencer = FindObjectOfType<HouseChildSequencer>();
        if (sequencer == null)
        {
            Debug.LogError("HouseChildSequencer not found in scene.");
            return;
        }

        sequencer.NotifyNpcInteracted(npc);
    }
}
