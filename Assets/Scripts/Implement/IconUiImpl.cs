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

        SetIconState(TipState.Hidden);
    }

    void Update()
    {
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

        Vector3 direction = targetCamera.transform.position - transform.position;
        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(-direction, targetCamera.transform.up);
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