using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FollowPathToObject : MonoBehaviour
{
    // public GameObject objectToPathTo;
    private Vector3 startPosition;

    public void Follow(GameObject objectToFollow) {
        startPosition = transform.position;
        
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(transform.DOMove(objectToFollow.transform.position, 3));
        mySequence.Append(transform.DOMove(startPosition, 3));
        mySequence.SetLoops(-1);


        // transform.DOPath(new Vector3[] {GetComponentInParent<GameObject>().transform.position, transform.position}, 1, PathType.CatmullRom).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).SetOptions(closePath: true);
    }
}
