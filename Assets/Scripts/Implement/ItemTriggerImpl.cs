using UnityEngine;

public class ItemTriggerImpl : MonoBehaviour, IItemTrigger
{
    [SerializeField] ItemTrigger itemTrigger;
    [SerializeField, HideInInspector] WebsiteUiImpl websiteUi;

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
        if (itemTrigger == null)
        {
            return;
        }

        if (websiteUi != null)
        {
            websiteUi.Show();
            return;
        }

        if (hasTriggered)
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
        // Website openers only respond to E / raycast Interact, not walk-in.
        if (websiteUi != null)
        {
            return;
        }

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
