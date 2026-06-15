using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] Camera targetCamera;

    void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    void LateUpdate()
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
}
