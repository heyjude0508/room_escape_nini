using UnityEngine;

public interface IIconUi
{
    public void FaceCamera();

    public bool IsInCheckRange { get; }

    public bool IsOwnedBy(Transform root);

    public void SetIconState(IconTipStatusEnum newState);

}
