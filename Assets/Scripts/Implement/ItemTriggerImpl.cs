using UnityEngine;

public class ItemTriggerImpl : MonoBehaviour, IItemTrigger
{
    [SerializeField] ItemTrigger itemTrigger;

    bool hasTriggered;

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
        if (itemTrigger == null)
        {
            return string.Empty;
        }

        return itemTrigger.itemActionDesc;
    }

    public void Interact()
    {
        if (hasTriggered || itemTrigger == null)
        {
            return;
        }

        hasTriggered = true;

        HouseChildSequencer sequencer = FindObjectOfType<HouseChildSequencer>();
        if (sequencer == null)
        {
            Debug.LogError("HouseChildSequencer not found in scene.");
            return;
        }

        sequencer.NotifyTriggerEntered(itemTrigger.id);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsPlayerCollider(other))
        {
            return;
        }

        Interact();
    }

    static bool IsPlayerCollider(Collider other)
    {
        if (other == null)
        {
            return false;
        }

        if (other.GetComponent<CharacterController>() != null)
        {
            return true;
        }

        return other.GetComponentInParent<FirstPersonController>() != null;
    }
}
