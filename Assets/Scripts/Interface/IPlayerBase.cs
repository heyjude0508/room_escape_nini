using UnityEngine;

public interface IPlayerBase
{
    void EventAimStart();

    void EventAimEnd();

    void EventInteract();

    string GetDescription();

    void Interact();
}
