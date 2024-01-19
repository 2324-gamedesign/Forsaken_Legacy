using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerEnemy : MonoBehaviour
{
    public Collider areaOfAttack;
    private bool canSee;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player"))
        {
            canSee = true;
            InvokeRepeating("Shoot", 0.5f, 5f);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player"))
        {
            canSee = false;
            CancelInvoke();
        }
    }

    private void Shoot()
    {
        if(canSee && GetComponent<Renderer>().isVisible){
            _animator.SetTrigger("Shoot");
        }
    }
}
