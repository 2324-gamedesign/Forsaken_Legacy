using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guardian : MonoBehaviour
{
    public float fieldOfViewAngle = 110f;
    public float fieldOfViewDistance = 10f;
    public float attackRange = 2f;

    protected float timerSinceLostTarget = 0.0f;
    private float timeToLostTarget = 1.0f;
    private Vector3 originalPosition;
    private bool targetInSight = false;
    private bool isPursuing = false;

    public float idleSpeed = 0f;
    public float walkSpeed = 1f;
    public float runSpeed = 2f;

    public GameObject _target = null;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        originalPosition = transform.position;
    }

    void Update()
    {
        _animator.SetFloat("Speed", _navMeshAgent.speed);
        if (_target)
        {
            CheckForPlayerInSight();
            if (targetInSight)
            {
                // Start pursuing the player
                StartPursuit();
            }
            else
            {
                return;
            }
        }
    }

    void CheckForPlayerInSight()
    {
        Vector3 toPlayer = _target.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, toPlayer);

        if (angle < fieldOfViewAngle * 0.5f)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, toPlayer.normalized, out hit, fieldOfViewDistance))
            {
                if (hit.collider.gameObject == _target)
                {
                    // Player is in sight
                    targetInSight = true;
                    timerSinceLostTarget = 0f;

                    // Check if the player is within attack range
                    if (toPlayer.magnitude < attackRange)
                    {
                        // Attack the player
                        TriggerAttack();
                    }
                }
                else
                {
                    // Player is not in sight
                    targetInSight = false;
                }
            }
        }

        // If the player is not in sight, start counting time since lost target
        if (!targetInSight && isPursuing)
        {
            timerSinceLostTarget += Time.deltaTime;

            // If enough time has passed, return to position
            if (timerSinceLostTarget >= timeToLostTarget)
            {
                StopPursuit();
                ReturnToOriginalPosition();
            }
        }
    }

    void StartPursuit()
    {

        // Start pursuing the player
        _navMeshAgent.SetDestination(_target.transform.position);
        _navMeshAgent.speed = runSpeed;
        isPursuing = true;
        Debug.Log("Pursuing player");
    }

    void StopPursuit()
    {
        isPursuing = false;
        _navMeshAgent.ResetPath();
    }

    void ReturnToOriginalPosition()
    {
        // Return to the original position if not pursuing
        if (!isPursuing)
        {
            _navMeshAgent.SetDestination(originalPosition);
            _navMeshAgent.speed = walkSpeed;
        }
    }

    void TriggerAttack()
    {
        // Trigger the attack animation or perform attack logic here
        _animator.SetTrigger("Attack");
    }

    void SetStop()
    {
        // isAttacking = true;
        _navMeshAgent.speed = idleSpeed;
    }
    void SetGo()
    {
        // isAttacking = false;
        _navMeshAgent.speed = runSpeed;
    }
}