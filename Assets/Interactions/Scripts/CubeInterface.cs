using UnityEngine;

public interface IInteractable
{
    public void Interact();

    public string GetDescription();

    public void EventAimStart();
    public void EventAimEnd();
}
