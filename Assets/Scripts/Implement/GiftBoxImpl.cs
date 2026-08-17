using System.Collections;
using UnityEngine;

public class GiftBoxImpl : MonoBehaviour
{
    const string KeyBlockName = "KeyBlock";
    const string LockInteractZoneName = "InteractZone";

    [SerializeField] Collider keyBlockCollider;
    [SerializeField] Collider keyCollider;
    [SerializeField] Transform drawer;
    [SerializeField] Vector3 drawerOpenLocalOffset = new Vector3(0f, 0f, 0.35f);
    [SerializeField] float drawerOpenDuration = 0.6f;
    [SerializeField] bool createMissingColliders = true;

    bool isOpen;
    IconUiImpl keyIconTip;
    Vector3 drawerClosedLocalPosition;
    Coroutine openRoutine;

    void Awake()
    {
        AutoFindReferences();
        ApplyClosedState();
    }

    public void AutoFindReferences()
    {
        if (drawer == null)
        {
            drawer = FindChildRecursive(transform, "Drawer");
        }

        if (drawer != null)
        {
            drawerClosedLocalPosition = drawer.localPosition;
        }

        ItemKeyImpl key = GetComponentInChildren<ItemKeyImpl>(true);
        if (key != null)
        {
            if (keyCollider == null)
            {
                keyCollider = key.GetComponent<Collider>();
            }

            keyIconTip = key.GetComponentInChildren<IconUiImpl>(true);
        }

        if (keyBlockCollider == null)
        {
            Transform block = FindChildRecursive(transform, KeyBlockName);
            if (block != null)
            {
                keyBlockCollider = block.GetComponent<Collider>();
            }
        }

        if (keyBlockCollider == null && createMissingColliders)
        {
            keyBlockCollider = CreateKeyBlockCollider();
        }

        EnsureLockInteractCollider();
    }

    public void OpenDrawer()
    {
        if (isOpen)
        {
            return;
        }

        isOpen = true;
        ApplyOpenState();

        if (drawer == null || drawerOpenDuration <= 0f)
        {
            if (drawer != null)
            {
                drawer.localPosition = drawerClosedLocalPosition + drawerOpenLocalOffset;
            }

            return;
        }

        if (openRoutine != null)
        {
            StopCoroutine(openRoutine);
        }

        openRoutine = StartCoroutine(AnimateDrawerOpen());
    }

    public bool IsOpen => isOpen;

    void ApplyClosedState()
    {
        if (keyBlockCollider != null)
        {
            keyBlockCollider.enabled = true;
        }

        if (keyCollider != null)
        {
            keyCollider.enabled = false;
        }

        if (keyIconTip != null)
        {
            keyIconTip.enabled = false;
            keyIconTip.gameObject.SetActive(false);
        }
    }

    void ApplyOpenState()
    {
        if (keyBlockCollider != null)
        {
            keyBlockCollider.enabled = false;
        }

        if (keyCollider != null)
        {
            keyCollider.enabled = true;
        }

        if (keyIconTip != null)
        {
            keyIconTip.gameObject.SetActive(true);
            keyIconTip.enabled = true;
        }
    }

    IEnumerator AnimateDrawerOpen()
    {
        Vector3 start = drawer.localPosition;
        Vector3 end = drawerClosedLocalPosition + drawerOpenLocalOffset;
        float elapsed = 0f;

        while (elapsed < drawerOpenDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / drawerOpenDuration);
            t = t * t * (3f - 2f * t);
            drawer.localPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }

        drawer.localPosition = end;
        openRoutine = null;
    }

    Collider CreateKeyBlockCollider()
    {
        Transform existing = FindChildRecursive(transform, KeyBlockName);
        GameObject blockObject;
        if (existing != null)
        {
            blockObject = existing.gameObject;
        }
        else
        {
            blockObject = new GameObject(KeyBlockName);
            blockObject.transform.SetParent(transform, false);
            blockObject.transform.localPosition = Vector3.zero;
            blockObject.transform.localRotation = Quaternion.identity;
            blockObject.transform.localScale = Vector3.one;
        }

        BoxCollider box = blockObject.GetComponent<BoxCollider>();
        if (box == null)
        {
            box = blockObject.AddComponent<BoxCollider>();
        }

        // Cover the closed lid / interior so top-down rays cannot reach the key,
        // while leaving the front face free for the combination lock.
        box.isTrigger = false;
        box.center = new Vector3(0f, 0.02f, -0.02f);
        box.size = new Vector3(0.85f, 0.22f, 0.55f);
        return box;
    }

    void EnsureLockInteractCollider()
    {
        Transform lockRoot = FindChildRecursive(transform, "Lock");
        if (lockRoot == null)
        {
            return;
        }

        Collider lockCollider = lockRoot.GetComponent<Collider>();
        if (lockCollider == null)
        {
            Transform zone = lockRoot.Find(LockInteractZoneName);
            if (zone != null)
            {
                lockCollider = zone.GetComponent<Collider>();
            }
        }

        if (lockCollider != null || !createMissingColliders)
        {
            return;
        }

        GameObject zoneObject = new GameObject(LockInteractZoneName);
        zoneObject.transform.SetParent(lockRoot, false);
        zoneObject.transform.localPosition = Vector3.zero;
        zoneObject.transform.localRotation = Quaternion.identity;
        zoneObject.transform.localScale = Vector3.one;

        BoxCollider box = zoneObject.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.center = Vector3.zero;
        box.size = new Vector3(0.45f, 0.55f, 0.35f);
    }

    static Transform FindChildRecursive(Transform parent, string name)
    {
        if (parent.name == name)
        {
            return parent;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform found = FindChildRecursive(parent.GetChild(i), name);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}
