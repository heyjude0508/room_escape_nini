using TMPro;
using UnityEngine;

public class PlayerActionImpl: MonoBehaviour

{
    [SerializeField] Camera mainCamera;

    [SerializeField] float InteractionRange;

    [SerializeField] GameObject UiInteraction;

    [SerializeField] TMP_Text UiInteractionText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindImpItem();
    }

    // Ñ°ÕÒ¹Ø¼üµÀ¾ß
    public void FindImpItem()
    {

        Ray ray = mainCamera.ViewportPointToRay(Vector3.one / 2);
        bool IsHit = false;
        UiInteraction.SetActive(false);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, InteractionRange))
        {

            IPlayerAction interactable = hitInfo.collider.GetComponent<IPlayerAction>();
            if (interactable != null)
            {
                IsHit = true;
                UiInteraction.SetActive(IsHit);
                UiInteractionText.text = interactable.GetDescription();
                 
                //interactable.EventAimStart();

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
}
