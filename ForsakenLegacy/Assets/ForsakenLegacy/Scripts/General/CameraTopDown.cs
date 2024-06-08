using System.Collections;
using UnityEngine;

public class CameraTopDown : MonoBehaviour
{
    [Header("Camera Settings")]
    [Range(5f, 50f)]
    public float VerticalOffset = 10f; // Height above the player
    [Range(0f, 360f)]
    public float Angle = 45f; // Angle of rotation around the player
    private readonly float _angle = 10f; // Distance behind the player

    [Header("Follow Settings")]
    [Range(0f, 1f)]
    public float FollowSmoothness = 0.3f; // Smoothness of camera follow
    [Range(0f, 1f)]
    public float FollowDelay = 0.5f; // Delay in following the player


    [Header("Target Settings")]
    public Transform player;

    private Vector3 refVelocity;
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
        float targetHeight = player.position.y + VerticalOffset;

        // Calculate the world position of the camera
        Vector3 worldPosition = (Vector3.forward * -_angle) + (Vector3.up * targetHeight);

        Vector3 rotatedVector = Quaternion.AngleAxis(Angle, Vector3.up) * worldPosition;

        Vector3 flatPlayerPos = player.position;
        flatPlayerPos.y = 0f;
        Vector3 finalPos = flatPlayerPos + rotatedVector;

        // Set the target position with a delay
        targetPosition = Vector3.Lerp(targetPosition, finalPos, Time.deltaTime / FollowDelay);

        // Smoothly move towards the target position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref refVelocity, FollowSmoothness);

        // Apply the smoothed position to the camera's transform
        transform.position = smoothedPosition;

        transform.LookAt(player);
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