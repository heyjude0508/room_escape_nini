using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAction
{
    void EventAimStart();

    void EventAimEnd();

    void EventInteract();

    // ���عؼ����ߵ�����
    string GetDescription();

    // �͵��߽��н���
    void Interact();

}