using TMPro;
using UnityEngine;

public class PlayerActionImpl : MonoBehaviour, IPlayerAction
{
    [Header("Camera Settings")]
    [SerializeField] Camera mainCamera;
    [SerializeField] float InteractionRange = 0.6f;
    [SerializeField] GameObject UiInteraction;
    [SerializeField] TMP_Text UiInteractionText;

    [Header("Walk Sound Settings")]
    [SerializeField] bool enableWalkSound = true;
    [SerializeField] AudioClip[] walkSounds;
    [SerializeField] AudioSource walkAudioSource;
    [SerializeField] float walkStepInterval = 0.45f;
    [SerializeField] float groundCheckDistance = 0.75f;

    static readonly KeyCode[] MovementKeys =
    {
        KeyCode.W,
        KeyCode.A,
        KeyCode.S,
        KeyCode.D,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow
    };

    float walkStepTimer;

    void Awake()
    {
        if (walkAudioSource == null)
        {
            walkAudioSource = GetComponent<AudioSource>();
            if (walkAudioSource == null)
            {
                walkAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        walkAudioSource.playOnAwake = false;
        walkAudioSource.loop = false;
        walkAudioSource.spatialBlend = 0f;
    }

    void Update()
    {
        DiscoverImpItem();
        HandleWalkSound();
    }

    public void DiscoverImpItem()
    {
        Ray ray = mainCamera.ViewportPointToRay(Vector3.one / 2);
        bool IsHit = false;
        UiInteraction.SetActive(false);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractionRange))
        {
            IBaseAction interactable = hitInfo.collider.GetComponentInParent<IBaseAction>();
            if (interactable != null)
            {
                IsHit = true;
                UiInteraction.SetActive(IsHit);
                UiInteractionText.text = interactable.GetDescription();

                interactable.EventAimStart();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
        }
        else
        {
            UiInteraction.SetActive(IsHit);
        }
    }

    public void HandleWalkSound()
    {
        if (!enableWalkSound || walkSounds == null || walkSounds.Length == 0)
        {
            return;
        }

        if (!IsMovementKeyHeld() || !IsGrounded())
        {
            walkStepTimer = 0f;
            return;
        }

        if (IsMovementKeyPressedThisFrame())
        {
            PlayRandomWalkSound();
            walkStepTimer = walkStepInterval;
            return;
        }

        walkStepTimer -= Time.deltaTime;
        if (walkStepTimer <= 0f)
        {
            PlayRandomWalkSound();
            walkStepTimer = walkStepInterval;
        }
    }

    public bool IsGrounded()
    {
        Vector3 origin = new Vector3(
            transform.position.x,
            transform.position.y - (transform.localScale.y * 0.5f),
            transform.position.z);

        return Physics.Raycast(origin, Vector3.down, groundCheckDistance);
    }

    public bool IsMovementKeyPressedThisFrame()
    {
        for (int i = 0; i < MovementKeys.Length; i++)
        {
            if (Input.GetKeyDown(MovementKeys[i]))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsMovementKeyHeld()
    {
        for (int i = 0; i < MovementKeys.Length; i++)
        {
            if (Input.GetKey(MovementKeys[i]))
            {
                return true;
            }
        }

        return false;
    }

    public void PlayRandomWalkSound()
    {
        if (walkAudioSource == null || walkSounds == null || walkSounds.Length == 0)
        {
            return;
        }

        AudioClip clip = walkSounds[Random.Range(0, walkSounds.Length)];
        if (clip != null)
        {
            walkAudioSource.PlayOneShot(clip);
        }
    }
}
