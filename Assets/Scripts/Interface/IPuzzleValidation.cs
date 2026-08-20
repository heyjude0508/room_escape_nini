using UnityEngine;

public interface IPuzzleValidation: IPuzzleBase
{
    void OpenInputCanvas();

    void ValidateCode(string inputCode);
}
