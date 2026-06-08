using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAction
{
    void EventAimStart();

    void EventAimEnd();

    void EventInteract();

    // 返回关键道具的描述
    string GetDescription();

    // 和道具进行交互
    void Interact();

}
