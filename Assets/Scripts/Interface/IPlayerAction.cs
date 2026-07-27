using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAction
{
    void DiscoverImpItem();

    void HandleWalkSound();

    void HandleCrouch();

    bool IsCrouching();

    void HandleBagItemSelection();

    bool IsGrounded();

    bool IsMovementKeyPressedThisFrame();

    bool IsMovementKeyHeld();

    void PlayRandomWalkSound();

}
