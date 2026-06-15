using UnityEngine;
using UnityEngine.UI;

public class InteractableTipController : MonoBehaviour
{
    enum TipState
    {
        Hidden,
        Point,
        Check
    }

    [SerializeField] float hintDistance = 1.3f;
    [SerializeField] float interactDistance = 0.7f;
    [SerializeField] Transform distanceTarget;
    [SerializeField] Camera targetCamera;
    [SerializeField] Image tipImage;
    [SerializeField] Sprite pointSprite;
    [SerializeField] Sprite checkSprite;

    TipState currentState = TipState.Hidden;

    void Awake()
    {
        if (tipImage == null)
        {
            tipImage = GetComponentInChildren<Image>(true);
        }

        if (distanceTarget == null && transform.parent != null)
        {
            distanceTarget = transform.parent;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        SetState(TipState.Hidden);
    }

    void Update()
    {
        if (distanceTarget == null || targetCamera == null || tipImage == null)
        {
            SetState(TipState.Hidden);
            return;
        }

        float distance = Vector3.Distance(targetCamera.transform.position, distanceTarget.position);

        if (distance > hintDistance)
        {
            SetState(TipState.Hidden);
        }
        else if (distance > interactDistance)
        {
            SetState(TipState.Point);
        }
        else
        {
            SetState(TipState.Check);
        }
    }

    void SetState(TipState newState)
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
                tipImage.sprite = pointSprite;
                tipImage.enabled = pointSprite != null;
                break;
            case TipState.Check:
                tipImage.sprite = checkSprite;
                tipImage.enabled = checkSprite != null;
                break;
        }
    }
}
