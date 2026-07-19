using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAction
{
    void DiscoverImpItem();

    void HandleWalkSound();

    bool IsGrounded();

    bool IsMovementKeyPressedThisFrame();

    bool IsMovementKeyHeld();

    void PlayRandomWalkSound();

}
