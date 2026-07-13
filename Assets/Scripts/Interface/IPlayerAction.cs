using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAction
{
    void EventAimStart();

    void EventAimEnd();

    void EventInteract();

    string GetDescription();

    void Interact();

}
