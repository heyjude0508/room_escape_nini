using UnityEngine;

public interface IPuzzleLock: IPuzzleBase
{
    bool CanSolve();

    void Unlock();

}
