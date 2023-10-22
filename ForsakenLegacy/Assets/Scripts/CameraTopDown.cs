using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CameraTopDown : MonoBehaviour
{

    public float height = 10f;
    public float distance = 20f;
    public float angle = 45f;
    private float smoothSpeed = 0.5f;
    public Transform player;
    
    private Vector3 refVelocity;


    // Start is called before the first frame update
    void Start()
    {
        HandleCamera();
    }

    // Update is called once per frame
    void Update() {
        HandleCamera();
    }

    protected virtual void HandleCamera()
    {
        if(!player){
            return;
        }

        Vector3 worldPosition = (Vector3.forward * -distance) + (Vector3.up * height);
        Debug.DrawLine(player.position, worldPosition, Color.red);

        Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPosition;
        Debug.DrawLine(player.position, rotatedVector, Color.green);


        Vector3 flatPlayerPos = player.position;
        flatPlayerPos.y = 0f;
        Vector3 finalPos = flatPlayerPos + rotatedVector;
        Debug.DrawLine(player.position, finalPos, Color.blue);

        transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref refVelocity, smoothSpeed);

        transform.LookAt(player.position);
    }
}
