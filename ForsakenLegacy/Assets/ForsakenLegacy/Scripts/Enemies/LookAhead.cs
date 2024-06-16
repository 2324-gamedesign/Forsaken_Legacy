using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LookAhead : MonoBehaviour
{
    private Vector3 previousPosition;
    private Vector3 previousDirection;
    private Tween tween;
    // Start is called before the first frame update
    void Start()
    {
        previousPosition = transform.position;
        previousDirection = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != previousPosition)
        {
            Vector3 direction = (transform.position - previousPosition).normalized;
            if(Vector3.Distance(direction, previousDirection) > 0.01f)
            {
                tween?.Kill();
                tween = transform.DOLookAt(transform.position + direction, 0.1f, AxisConstraint.Y); 
                previousDirection = direction;
            };
            previousPosition = transform.position;
        }
    }
}
