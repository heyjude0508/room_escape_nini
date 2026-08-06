using UnityEngine;
using UnityEngine.UI;

public class IconUiImpl : MonoBehaviour, IIconUi
{
    [SerializeField] Camera targetCamera;
    [SerializeField] Image tipImage;
    [SerializeField] Sprite circleSprite;
    [SerializeField] Sprite checkSprite;
    [SerializeField] float hintDistance = 1.2f;
    [SerializeField] float interactDistance = 0.6f;
    Vector2 smallsize = new Vector2(0.0005f, 0.0005f);
    Vector2 largeSize = new Vector2(0.01f, 0.01f);
    Transform distanceTarget;
    Transform positionAnchor;
    Transform ownerRoot;
    Vector3 anchorOffset;
    bool useDetachedBillboard;

    enum TipState
    {
        Hidden,
        Point,
        Check
    }

    TipState currentState = TipState.Hidden;

    void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (tipImage == null)
        {
            tipImage = GetComponentInChildren<Image>(true);
        }

        if (distanceTarget == null && transform.parent != null)
        {
            distanceTarget = transform.parent;
        }

        positionAnchor = distanceTarget != null ? distanceTarget : transform.parent;
        ownerRoot = transform.parent;
        TryDetachFromSkewedHierarchy();

        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.worldCamera == null && targetCamera != null)
        {
            canvas.worldCamera = targetCamera;
        }

        SetIconState(TipState.Hidden);
    }

    // Interactions uses (60, 60, 1) scale. Unity cannot preserve world rotation on children
    // of non-uniformly scaled parents, so billboards there only yaw instead of facing the camera.
    void TryDetachFromSkewedHierarchy()
    {
        if (!HasNonUniformScaleInParents(transform))
        {
            return;
        }

        if (positionAnchor != null)
        {
            anchorOffset = transform.position - positionAnchor.position;
        }

        Vector3 worldPosition = transform.position;
        float uniformScale = GetMedianComponent(transform.lossyScale);
        transform.SetParent(null, false);
        transform.position = worldPosition;
        transform.localScale = Vector3.one * uniformScale;
        useDetachedBillboard = true;

        if (positionAnchor != null)
        {
            transform.position = positionAnchor.position + anchorOffset;
        }
    }

    static bool HasNonUniformScaleInParents(Transform target)
    {
        Transform current = target.parent;
        while (current != null)
        {
            Vector3 scale = current.localScale;
            if (!Mathf.Approximately(scale.x, scale.y) || !Mathf.Approximately(scale.y, scale.z))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    static float GetMedianComponent(Vector3 values)
    {
        float[] axes = { values.x, values.y, values.z };
        System.Array.Sort(axes);
        return Mathf.Max(axes[1], 0.001f);
    }

    void LateUpdate()
    {
        if (useDetachedBillboard && positionAnchor != null)
        {
            transform.position = positionAnchor.position + anchorOffset;
        }

        FaceCamera();

        if (distanceTarget == null || targetCamera == null || tipImage == null)
        {
            SetIconState(TipState.Hidden);
            return;
        }

        float distance = Vector3.Distance(targetCamera.transform.position, distanceTarget.position);

        if (distance > hintDistance)
        {
            SetIconState(TipState.Hidden);
        }
        else if (distance > interactDistance)
        {
            SetIconState(TipState.Point);
        }
        else
        {
            SetIconState(TipState.Check);
        }
    }

    public void FaceCamera()
    {
        if (targetCamera == null)
        {
            return;
        }

        // World-space Canvas draws on its -Z face; +Z must point away from the camera.
        Vector3 awayFromCamera = transform.position - targetCamera.transform.position;
        if (awayFromCamera.sqrMagnitude < 0.0001f)
        {
            awayFromCamera = targetCamera.transform.forward;
        }

        transform.rotation = Quaternion.LookRotation(awayFromCamera, targetCamera.transform.up);
    }

    public bool IsOwnedBy(Transform root)
    {
        if (ownerRoot == null || root == null)
        {
            return false;
        }

        return ownerRoot == root || ownerRoot.IsChildOf(root);
    }

    void SetIconState(TipState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        switch (newState)
        {
            case TipState.Hidden:
                tipImage.enabled = false;
                break;
            case TipState.Point:
                tipImage.sprite = circleSprite;
                tipImage.rectTransform.sizeDelta = smallsize;
                tipImage.enabled = circleSprite != null;
                break;
            case TipState.Check:
                tipImage.sprite = checkSprite;
                tipImage.rectTransform.sizeDelta = largeSize;
                tipImage.enabled = checkSprite != null;
                break;
        }
    }

}
