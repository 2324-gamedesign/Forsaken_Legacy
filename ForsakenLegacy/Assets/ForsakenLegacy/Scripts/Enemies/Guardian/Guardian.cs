using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.AI;

namespace ForsakenLegacy
{
    public class Guardian : MonoBehaviour
    {
        public float fieldOfViewAngle = 110f;
        public float fieldOfViewDistance = 10f;
        public Collider attackRange;

        private Vector3 originalPosition;
        protected float timerSinceLostTarget = 0.0f;
        private float timeToLostTarget = 1.0f;
        private bool targetInSight = false;
        private bool isPursuing = false;
        private bool isAttacking = false;

        public float idleSpeed = 0f;
        public float walkSpeed = 2f;
        public float runSpeed = 4f;

        private float minDistanceToPlayer = 0.8f;
        private bool isInsideAttackRange = false;

        public GameObject _target = null;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        public Collider weaponCollider;

        public MMFeedbacks feedbackAttack;


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
                if(!isAttacking) CheckForPlayerInSight();
                if (targetInSight && !isAttacking)
                {
                    // Start pursuing the player
                    StartPursuit();
                }
            }

            if (isAttacking)
            {
                Vector3 toPlayer = _target.transform.position - transform.position;
                float distanceToPlayer = toPlayer.magnitude;

                if (distanceToPlayer < minDistanceToPlayer)
                {
                    Vector3 newPosition = _target.transform.position - toPlayer.normalized * minDistanceToPlayer;
                    transform.position = newPosition;
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
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        // Player is in sight
                        targetInSight = true;
                        timerSinceLostTarget = 0f;
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
            _navMeshAgent.isStopped = false;
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


        private void OnTriggerEnter(Collider other) {
            if(attackRange)
            {
                if(other == attackRange)
                {
                    Debug.Log("Attack");
                    isInsideAttackRange = true;

                    _navMeshAgent.ResetPath();
                    InvokeRepeating("TriggerAttack", 0.5f, 2f);
                    _navMeshAgent.isStopped = true;
                }
            }
            else return;
        }

        private void OnTriggerExit(Collider other) {
            if(other == attackRange)
            {
                CancelInvoke();
                _navMeshAgent.isStopped = false;
                isInsideAttackRange = false;
            }
        }

        void TriggerAttack()
        {
            int indexAttack;
            indexAttack = Random.Range(0, 2);

            feedbackAttack.PlayFeedbacks();

            // Trigger the attack animation or perform attack logic here
            if(indexAttack == 0) _animator.SetTrigger("Attack");
            if(indexAttack == 1) _animator.SetTrigger("Combo");
        }


        // Methods Triggered in Animations
        void SetStop()
        {
            isAttacking = true;
            _navMeshAgent.speed = idleSpeed;
        }
        void SetGo()
        {
            if(!isInsideAttackRange)
            {
                isAttacking = false;
            }
        }

        void SetRootMotion()
        {
            _animator.applyRootMotion = true;
        }
        void UnsetRootMotion()
        {
            _animator.applyRootMotion = false;
        }

        void ColliderWeaponOn()
        {
            weaponCollider.enabled = true;
        }
        void ColliderWeaponOff()
        {
            weaponCollider.enabled = false;
        }
    }
}
