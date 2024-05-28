using System.Collections;
using UnityEngine;

public class CameraTopDown : MonoBehaviour
{
    public float height = 10f;
    public float distance = 20f;
    public float angle = 45f;
    public float smoothSpeed = 0f;
    public Transform player;
    private Vector3 refVelocity;

    // Add a delay factor to control the delay between player movement and camera follow
    public float followDelay = 0.5f;
    private Vector3 targetPosition;

    private DitherToShowPlayer _ditherToShowPlayer = null;

    void Start()
    {
        HandleCamera();
    }

    void Update()
    {
        HandleCamera();
        HandleDither();
    }

    protected virtual void HandleCamera()
    {
        if (!player)
        {
            return;
        }

        // Calculate the target height based on the player's position
        float targetHeight = player.position.y + height;

        // Calculate the world position of the camera
        Vector3 worldPosition = (Vector3.forward * - distance) + (Vector3.up * targetHeight);

        Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPosition;

        Vector3 flatPlayerPos = player.position;
        flatPlayerPos.y = 0f;
        Vector3 finalPos = flatPlayerPos + rotatedVector;

        // Set the target position with a delay
        targetPosition = Vector3.Lerp(targetPosition, finalPos, Time.deltaTime /    followDelay);

        // Smoothly move towards the target position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition,   ref refVelocity, smoothSpeed);

        // Apply the smoothed position to the camera's transform
        transform.position = smoothedPosition;
    }

    private void HandleDither()
    {
        if(player != null)
        {
            Vector3 dir = player.position - transform.position;
            Ray ray = new Ray(transform.position, dir.normalized);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider == null || hit.collider.gameObject == player)
                {
                    if(_ditherToShowPlayer != null) _ditherToShowPlayer.Dither = false;
                }
                else if(hit.collider.gameObject.GetComponent<DitherToShowPlayer>())
                {
                    _ditherToShowPlayer = hit.collider.gameObject.GetComponent<DitherToShowPlayer>();
                    _ditherToShowPlayer.Dither = true;
                }
                else
                {
                    if(_ditherToShowPlayer != null) _ditherToShowPlayer.Dither = false;
                }
            }
        }
    }
}