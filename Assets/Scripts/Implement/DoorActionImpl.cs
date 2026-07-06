using UnityEngine;
using UnityEngine.Events;

public class DoorActionImpl : MonoBehaviour, IDoorAction
{
    [SerializeField] Animation doorAnimation;
    [SerializeField] string openAnimationName = "DoorRoomsWide_open";
    [SerializeField] bool isLocked;
    [SerializeField] string requiredKeyID;
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip lockSound;
    [SerializeField] AudioClip closeSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] MonoBehaviour interactableSound;
    [SerializeField] Collider doorBlocker;
    [SerializeField] UnityEvent onDoorOpened;

    [SerializeField] string description = "Press E to open the door.";
    [SerializeField] string lockedDescription = "The door is locked.";

    IBagManagement bag;
    bool isOpen;

    void Awake()
    {
        if (doorAnimation == null)
        {
            doorAnimation = GetComponent<Animation>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (doorBlocker == null)
        {
            doorBlocker = FindSolidCollider();
        }
    }

    void Start()
    {
        bag = BagManagementImpl.Instance;
    }

    public void EventAimStart()
    {
    }

    public void EventAimEnd()
    {
    }

    public void EventInteract()
    {
    }

    public string GetDescription()
    {
        if (isOpen)
        {
            return string.Empty;
        }

        return isLocked && !CanOpen() ? lockedDescription : description;
    }

    public void Interact()
    {
        if (isOpen)
        {
            return;
        }

        if (!CanOpen())
        {
            PlaySound(lockSound);
            Debug.Log("需要钥匙才能打开这扇门。");
            return;
        }

        if (isLocked && bag != null && !string.IsNullOrEmpty(requiredKeyID))
        {
            bag.RemoveItem(requiredKeyID);
        }

        PlayOpenAnimation();
        PlaySound(openSound);
        onDoorOpened?.Invoke();

        if (doorBlocker != null)
        {
            doorBlocker.enabled = false;
        }

        isOpen = true;
    }

    bool CanOpen()
    {
        if (!isLocked)
        {
            return true;
        }

        if (bag == null || string.IsNullOrEmpty(requiredKeyID))
        {
            return false;
        }

        return bag.HasItem(requiredKeyID);
    }

    void PlayOpenAnimation()
    {
        if (doorAnimation == null)
        {
            Debug.LogWarning($"门 [{name}] 缺少 Animation 组件。");
            return;
        }

        if (!string.IsNullOrEmpty(openAnimationName))
        {
            doorAnimation.Play(openAnimationName);
            return;
        }

        doorAnimation.Play();
    }

    public void PlaySound(AudioClip sound)
    {
        if (interactableSound is IInteractableSound customSound)
        {
            customSound.PlayInteractSound();
            return;
        }

        if (sound == null)
        {
            return;
        }

        if (audioSource != null)
        {
            audioSource.PlayOneShot(sound);
            return;
        }

        AudioSource.PlayClipAtPoint(sound, transform.position);
    }
    
    Collider FindSolidCollider()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            if (!collider.isTrigger)
            {
                return collider;
            }
        }

        return null;
    }
}
