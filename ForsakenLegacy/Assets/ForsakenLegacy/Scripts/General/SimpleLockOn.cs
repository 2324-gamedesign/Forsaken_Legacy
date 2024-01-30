using UnityEngine;

public class HorizontalBillboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    private void Start()
    {
        // Find the main camera transform
        mainCameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        // Calculate the new rotation to face the camera horizontally
        Vector3 lookAtPos = new Vector3(mainCameraTransform.position.x, transform.position.y, mainCameraTransform.position.z);
        transform.LookAt(lookAtPos);
    }
}