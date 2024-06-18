using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ForsakenLegacy;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public Vector3 EndPos;
    private Vector3 _startPos;
    private bool _atStart = true;
    public float Duration;
    public float WaitTime;
    private CapsuleCollider _player;

    [Header("Vines")]
    public GrowVine VineBody;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Edea").GetComponent<CapsuleCollider>();
        _startPos = transform.localPosition;
        StartCoroutine(Move());
    }
    private IEnumerator Move()
    {
        yield return new WaitForSeconds(WaitTime);
        if (_atStart)
        {
            _atStart = false;
            VineBody.Grow();
            transform.DOLocalMove(EndPos, Duration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                StartCoroutine(Move());
            });
        }
        else
        {
            _atStart = true;
            VineBody.Shrink();
            transform.DOLocalMove(_startPos, Duration).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                StartCoroutine(Move());
            });
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other = _player)
        {
            other.transform.SetParent(transform);
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other = _player)
        {
            other.transform.SetParent(null);
        }
    }
}
