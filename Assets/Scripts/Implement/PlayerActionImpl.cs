using TMPro;
using UnityEngine;

public class PlayerActionImpl : MonoBehaviour, IPlayerAction
{
    [Header("Camera Settings")]
    [SerializeField] Camera mainCamera;
    [SerializeField] float interactionRaycastRange = 1.3f;
    [SerializeField] float InteractionRange = 0.6f;
    [SerializeField] GameObject UiInteraction;
    [SerializeField] TMP_Text UiInteractionText;

    [Header("Bag Settings")]
    [SerializeField] BagUiImpl bagUi;

    [Header("Walk Sound Settings")]
    [SerializeField] bool enableWalkSound = true;
    [SerializeField] AudioClip[] walkSounds;
    [SerializeField] AudioSource walkAudioSource;
    [SerializeField] float walkStepInterval = 0.45f;
    [SerializeField] float groundCheckDistance = 0.75f;

    [Header("Crouch Settings")]
    [SerializeField] bool enableCrouch = true;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftShift;
    [SerializeField] float crouchHeightRatio = 0.55f;

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
    Vector3 originalScale;
    bool isCrouched;
    bool sprintEnabledBeforeCrouch = true;
    FirstPersonController firstPersonController;

    void Awake()
    {
        originalScale = transform.localScale;
        firstPersonController = GetComponent<FirstPersonController>();

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

        if (bagUi == null)
        {
            bagUi = FindObjectOfType<BagUiImpl>();
        }
    }

    void Update()
    {
        HandleCrouch();

        if (bagUi != null && bagUi.IsBagOpen())
        {
            HandleBagItemSelection();
        }
        else
        {
            DiscoverImpItem();
        }

        HandleWalkSound();
    }

    void OnDisable()
    {
        if (isCrouched)
        {
            SetCrouched(false);
        }
    }

    public void HandleBagItemSelection()
    {
        if (bagUi == null || !bagUi.IsBagOpen())
        {
            return;
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        bagUi.TrySelectItemAtScreenPoint(Input.mousePosition);
    }

    public void DiscoverImpItem()
    {
        if (mainCamera == null)
        {
            return;
        }

        Ray ray = mainCamera.ViewportPointToRay(Vector3.one / 2);
        bool IsHit = false;
        if (UiInteraction != null)
        {
            UiInteraction.SetActive(false);
        }

        if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionRaycastRange))
        {
            IPlayerBase interactable = hitInfo.collider.GetComponentInParent<IPlayerBase>();
            if (interactable != null && CanInteract(interactable))
            {
                IsHit = true;
                if (UiInteraction != null)
                {
                    UiInteraction.SetActive(IsHit);
                }

                if (UiInteractionText != null)
                {
                    UiInteractionText.text = interactable.GetDescription();
                }

                interactable.EventAimStart();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
        }
        else if (UiInteraction != null)
        {
            UiInteraction.SetActive(IsHit);
        }
    }

    public void HandleCrouch()
    {
        if (!enableCrouch)
        {
            return;
        }

        bool wantCrouch = Input.GetKey(crouchKey);
        if (wantCrouch == isCrouched)
        {
            return;
        }

        SetCrouched(wantCrouch);
    }

    public bool IsCrouching()
    {
        return isCrouched;
    }

    void SetCrouched(bool crouch)
    {
        isCrouched = crouch;

        float scaleY = crouch
            ? originalScale.y * crouchHeightRatio
            : originalScale.y;

        transform.localScale = new Vector3(originalScale.x, scaleY, originalScale.z);

        if (firstPersonController == null)
        {
            return;
        }

        if (crouch)
        {
            sprintEnabledBeforeCrouch = firstPersonController.enableSprint;
            firstPersonController.enableSprint = false;
            return;
        }

        firstPersonController.enableSprint = sprintEnabledBeforeCrouch;
    }

    public void HandleWalkSound()
    {
        if (!enableWalkSound || walkSounds == null || walkSounds.Length == 0)
        {
            return;
        }

        if (firstPersonController != null && !firstPersonController.playerCanMove)
        {
            walkStepTimer = 0f;
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

    bool CanInteract(IPlayerBase interactable)
    {
        MonoBehaviour interactableBehaviour = interactable as MonoBehaviour;
        if (interactableBehaviour == null || mainCamera == null)
        {
            return false;
        }

        IconUiImpl iconUi = IconUiImpl.FindForInteractable(interactableBehaviour.transform);
        if (iconUi != null)
        {
            return iconUi.IsInCheckRange;
        }

        float distance = Vector3.Distance(mainCamera.transform.position, interactableBehaviour.transform.position);
        return distance <= InteractionRange;
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
