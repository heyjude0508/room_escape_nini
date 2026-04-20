using TMPro;
using UnityEngine;

public class CubeImpl : MonoBehaviour

{
    public Camera mainCamera;

    public float InteractionRange;

    public GameObject UI_Interaction;

    public TMP_Text UI_InteractionText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InteractionRay();
    }

    private void InteractionRay()
    {

        Ray ray = mainCamera.ViewportPointToRay(Vector3.one / 2);
        RaycastHit hitInfo;
        bool IsHit = false;
        UI_Interaction.SetActive(false);

        if (Physics.Raycast(ray, out hitInfo, InteractionRange))
        {

            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                IsHit = true;
                UI_Interaction.SetActive(true);
                UI_InteractionText.text = interactable.GetDescription();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
        }
        else
        {
            UI_Interaction.SetActive(false);
        }
    }
}
