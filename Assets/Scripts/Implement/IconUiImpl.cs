using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconUiImpl : MonoBehaviour, IIconUi
{
    const string ActPanelName = "ActPanel";
    const string EKeyName = "EKey";
    const string NoEKeyName = "NoEKey";

    static readonly List<IconUiImpl> activeInstances = new List<IconUiImpl>();
    static Transform actPanelEKey;
    static Transform actPanelNoEKey;
    static bool actPanelKeysResolved;

    [SerializeField] Camera targetCamera;
    [SerializeField] Image tipImage;
    [SerializeField] Sprite circleSprite;
    [SerializeField] Sprite checkSprite;
    [SerializeField] float hintDistance = 1.2f;
    [SerializeField] float interactDistance = 0.6f;
    [SerializeField] float worldScale = 1f;
    [SerializeField] float cameraPull = 0.06f;
    Vector2 smallsize = new Vector2(0.0005f, 0.0005f);
    Vector2 largeSize = new Vector2(0.01f, 0.01f);
    Transform distanceTarget;
    Transform positionAnchor;
    Transform ownerRoot;
    Vector3 localAnchorOffset;
    Vector3 authoredLocalPosition;
    bool useDetachedBillboard;

    IconTipStatusEnum currentState = IconTipStatusEnum.ICON_TIP_HIDDEN;

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
        authoredLocalPosition = transform.localPosition;
        TryDetachFromSkewedHierarchy();

        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.worldCamera == null && targetCamera != null)
        {
            canvas.worldCamera = targetCamera;
        }

        SetIconState(IconTipStatusEnum.ICON_TIP_HIDDEN);
    }

    void OnEnable()
    {
        if (!activeInstances.Contains(this))
        {
            activeInstances.Add(this);
        }
    }

    void OnDisable()
    {
        activeInstances.Remove(this);
    }

    void LateUpdate()
    {
        ApplyWorldPosition();
        FaceCamera();

        if (distanceTarget == null || targetCamera == null || tipImage == null)
        {
            SetIconState(IconTipStatusEnum.ICON_TIP_HIDDEN);
            return;
        }

        float distance = Vector3.Distance(targetCamera.transform.position, distanceTarget.position);

        if (distance > hintDistance || IsOccludedBySolidGeometry())
        {
            SetIconState(IconTipStatusEnum.ICON_TIP_HIDDEN);
        }
        else if (distance > interactDistance)
        {
            SetIconState(IconTipStatusEnum.ICON_TIP_POINT);
        }
        else
        {
            SetIconState(IconTipStatusEnum.ICON_TIP_CHECK);
        }
    }

    bool IsOccludedBySolidGeometry()
    {
        if (targetCamera == null || distanceTarget == null)
        {
            return false;
        }

        Vector3 origin = targetCamera.transform.position;
        Vector3 toTarget = distanceTarget.position - origin;
        float distance = toTarget.magnitude;
        if (distance <= 0.0001f)
        {
            return false;
        }

        if (!Physics.Raycast(
                origin,
                toTarget / distance,
                out RaycastHit hit,
                distance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        Transform hitTransform = hit.transform;
        if (ownerRoot != null
            && (hitTransform == ownerRoot
                || hitTransform.IsChildOf(ownerRoot)
                || ownerRoot.IsChildOf(hitTransform)))
        {
            return false;
        }

        if (distanceTarget != null
            && (hitTransform == distanceTarget
                || hitTransform.IsChildOf(distanceTarget)
                || distanceTarget.IsChildOf(hitTransform)))
        {
            return false;
        }

        return true;
    }

    void ApplyWorldPosition()
    {
        Vector3 basePosition;
        if (useDetachedBillboard && positionAnchor != null)
        {
            // Follow anchor pose (incl. door rotation), not a frozen world offset.
            basePosition = positionAnchor.TransformPoint(localAnchorOffset);
        }
        else if (transform.parent != null)
        {
            basePosition = transform.parent.TransformPoint(authoredLocalPosition);
        }
        else
        {
            basePosition = transform.position;
        }

        // Pull toward camera so world-space tips stay in front of nearby meshes.
        if (targetCamera != null && cameraPull > 0f)
        {
            Vector3 toCamera = targetCamera.transform.position - basePosition;
            if (toCamera.sqrMagnitude > 0.0001f)
            {
                basePosition += toCamera.normalized * cameraPull;
            }
        }

        transform.position = basePosition;
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

    public bool IsInCheckRange
    {
        get
        {
            if (distanceTarget == null || targetCamera == null)
            {
                return false;
            }

            float distance = Vector3.Distance(targetCamera.transform.position, distanceTarget.position);
            return distance <= interactDistance;
        }
    }

    public bool BelongsToItemKey
    {
        get
        {
            return ownerRoot != null && ownerRoot.GetComponent<ItemKeyImpl>() != null;
        }
    }

    public static IconUiImpl FindForInteractable(Transform interactableTransform)
    {
        if (interactableTransform == null)
        {
            return null;
        }

        // ItemKey pickup must use its own tip (dot → magnifier), not a parent Lock tip.
        ItemKeyImpl itemKey = interactableTransform.GetComponent<ItemKeyImpl>();
        if (itemKey == null)
        {
            itemKey = interactableTransform.GetComponentInParent<ItemKeyImpl>();
        }

        if (itemKey != null)
        {
            return FindTipForItemKey(itemKey.transform);
        }

        IconUiImpl preferredLockTip = null;
        IconUiImpl fallbackTip = null;
        IconUiImpl[] tips = interactableTransform.GetComponentsInChildren<IconUiImpl>(true);
        for (int i = 0; i < tips.Length; i++)
        {
            IconUiImpl tip = tips[i];
            if (tip == null || tip.BelongsToItemKey)
            {
                continue;
            }

            if (tip.ownerRoot != null && tip.ownerRoot.name == "Lock")
            {
                preferredLockTip = tip;
                break;
            }

            if (fallbackTip == null)
            {
                fallbackTip = tip;
            }
        }

        if (preferredLockTip != null)
        {
            return preferredLockTip;
        }

        if (fallbackTip != null)
        {
            return fallbackTip;
        }

        for (int i = activeInstances.Count - 1; i >= 0; i--)
        {
            IconUiImpl candidate = activeInstances[i];
            if (candidate == null || candidate.BelongsToItemKey)
            {
                continue;
            }

            if (candidate.IsOwnedBy(interactableTransform))
            {
                return candidate;
            }
        }

        return null;
    }

    static IconUiImpl FindTipForItemKey(Transform keyRoot)
    {
        IconUiImpl[] tips = keyRoot.GetComponentsInChildren<IconUiImpl>(true);
        for (int i = 0; i < tips.Length; i++)
        {
            if (tips[i] != null)
            {
                return tips[i];
            }
        }

        for (int i = activeInstances.Count - 1; i >= 0; i--)
        {
            IconUiImpl candidate = activeInstances[i];
            if (candidate != null && candidate.IsOwnedBy(keyRoot))
            {
                return candidate;
            }
        }

        return null;
    }

    // Interactions uses (60, 60, 1) scale. Unity cannot preserve world rotation on children
    // of non-uniformly scaled parents, so billboards there only yaw instead of facing the camera.
    // Detach to root, but keep a local-space offset so moving/rotating parents (e.g. Door_R) still carry the tip.
    void TryDetachFromSkewedHierarchy()
    {
        if (!HasNonUniformScaleInParents(transform))
        {
            return;
        }

        if (positionAnchor != null)
        {
            localAnchorOffset = positionAnchor.InverseTransformPoint(transform.position);
        }

        Vector3 worldPosition = transform.position;
        transform.SetParent(null, false);
        transform.position = worldPosition;
        transform.localScale = Vector3.one * worldScale;
        useDetachedBillboard = true;

        if (positionAnchor != null)
        {
            transform.position = positionAnchor.TransformPoint(localAnchorOffset);
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

    public bool IsOwnedBy(Transform root)
    {
        if (ownerRoot == null || root == null)
        {
            return false;
        }

        return ownerRoot == root || ownerRoot.IsChildOf(root);
    }

    public void SetIconState(IconTipStatusEnum newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        switch (newState)
        {
            case IconTipStatusEnum.ICON_TIP_HIDDEN:
                tipImage.enabled = false;
                break;
            case IconTipStatusEnum.ICON_TIP_POINT:
                tipImage.sprite = circleSprite;
                tipImage.rectTransform.sizeDelta = smallsize;
                tipImage.enabled = circleSprite != null;
                break;
            case IconTipStatusEnum.ICON_TIP_CHECK:
                tipImage.sprite = checkSprite;
                tipImage.rectTransform.sizeDelta = largeSize;
                tipImage.enabled = checkSprite != null;
                break;
        }
    }

    public static void SetCanInteract(bool canInteract)
    {
        CacheActPanelKeys();
        if (actPanelEKey != null)
        {
            actPanelEKey.gameObject.SetActive(canInteract);
        }

        if (actPanelNoEKey != null)
        {
            actPanelNoEKey.gameObject.SetActive(!canInteract);
        }
    }

    public static void ShowDefaultEKey()
    {
        SetCanInteract(true);
    }

    static void CacheActPanelKeys()
    {
        if (actPanelKeysResolved && actPanelEKey != null && actPanelNoEKey != null)
        {
            return;
        }

        actPanelKeysResolved = true;
        Transform actPanel = FindActPanel();
        if (actPanel == null)
        {
            return;
        }

        actPanelEKey = actPanel.Find(EKeyName);
        actPanelNoEKey = actPanel.Find(NoEKeyName);
    }

    static Transform FindActPanel()
    {
        Canvas[] canvases = Object.FindObjectsOfType<Canvas>(true);
        for (int i = 0; i < canvases.Length; i++)
        {
            Transform found = FindChildRecursive(canvases[i].transform, ActPanelName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    static Transform FindChildRecursive(Transform parent, string childName)
    {
        if (parent == null)
        {
            return null;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == childName)
            {
                return child;
            }

            Transform nested = FindChildRecursive(child, childName);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }

}
