using UnityEngine;

public interface IPuzzlePosition: IPuzzleBase
{
    bool CanSolve();

    void PlaceItem();

}
