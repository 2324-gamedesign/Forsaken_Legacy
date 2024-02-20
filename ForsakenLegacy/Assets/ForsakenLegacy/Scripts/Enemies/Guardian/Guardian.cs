using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

namespace ForsakenLegacy
{
    public class Guardian : MonoBehaviour
    {
        public enum Behavior
        {
            Path,
            Still
        }

        public Behavior behavior;
        public Transform[] waypoints;
        private Vector3[] path;
        public PathType pathType;
        public float duration;

        public float fieldOfViewAngle = 110f;
        public float fieldOfViewDistance = 10f;
        public Collider attackRange;

        private Vector3 originalPosition;
        protected float timerSinceLostTarget = 0.0f;
        private float timeToLostTarget = 1.0f;
        private bool targetInSight = false;

        public bool isPursuing = false;
        public bool isAttacking = false;
        public bool isReturning = false;

        public float idleSpeed = 0f;
        public float walkSpeed = 2f;
        public float runSpeed = 4f;

        private float minDistanceToPlayer = 0.9f;
        private bool isInsideAttackRange = false;

        public GameObject _target = null;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        public Collider weaponCollider;

        public MMFeedbacks feedbackAttack;


        void Start()
        {
            path = new Vector3[waypoints.Length];
            for (int i = 0; i < waypoints.Length; i++) 
            {
                path[i] = waypoints[i].position;
            };

            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            originalPosition = transform.position;

            if(behavior == Behavior.Path){Patrol();}
            if(behavior == Behavior.Still){_navMeshAgent.speed = idleSpeed;}
        }

        void Update()
        {
            _animator.SetFloat("Speed", _navMeshAgent.speed);
            if (_target && !_target.GetComponent<HealthSystem>().isDead)
            {
                if(!isAttacking) CheckForPlayerInSight();
                if (targetInSight && !isAttacking)
                {
                    // Start pursuing the player
                    StartPursuit();
                }
            }

            //If it's in attack, avoid enemy penetrating into player
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

            //if the enemy returned to position start patrolling again
            if(IsPathComplete() && isReturning)
            {
                if(behavior == Behavior.Path)
                {
                    Patrol();
                }
                else if(behavior == Behavior.Still)
                {
                    _navMeshAgent.speed = idleSpeed;
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


        //Method that handles initial patrolling
        private void Patrol()
        {
            HandleLookAhead(true);
            _navMeshAgent.speed = walkSpeed;
            transform.DOPath(path, duration, pathType, PathMode.Ignore, 10, Color.red).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).SetOptions(closePath: true);
        }

        public void StartPursuit()
        {
            HandleLookAhead(false);
            DOTween.KillAll();
            isReturning = false;

            // Start pursuing the player
            _navMeshAgent.SetDestination(_target.transform.position);
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = runSpeed;
            isPursuing = true;
        }

        void StopPursuit()
        {
            isReturning = false;
            isPursuing = false;
            _navMeshAgent.ResetPath();
        }

        void ReturnToOriginalPosition()
        {
            DOTween.KillAll();

            // Return to the original position if not pursuing
            if (!isPursuing)
            {
                isReturning = true;
                _navMeshAgent.SetDestination(originalPosition);
                _navMeshAgent.speed = walkSpeed;
            }
        }


        private void OnTriggerEnter(Collider other) {
            if(attackRange)
            {
                if(other == attackRange && !_target.GetComponent<HealthSystem>().isDead)
                {
                    HandleLookAhead(false);
                    DOTween.KillAll();
                    StopPursuit();

                    InvokeRepeating("TriggerAttack", 0.1f, 2f);
                    _navMeshAgent.isStopped = true;
                }
            }
            else return;
        }

        private void OnTriggerExit(Collider other) {
            if(other == attackRange)
            {
                CancelInvoke();
                _animator.Play("Idle-Walk-Run");
                StartPursuit();
                isInsideAttackRange = false;
            }
        }

        void TriggerAttack()
        {
            if(!_target.GetComponent<HealthSystem>().isDead)
            {
                int indexAttack;
                indexAttack = Random.Range(0, 2);

                feedbackAttack.PlayFeedbacks();

                // Trigger the attack animation or perform attack logic here
                if(indexAttack == 0) _animator.SetTrigger("Attack");
                if(indexAttack == 1) _animator.SetTrigger("Combo"); 
            }
            else 
            {
                CancelInvoke();
                _animator.Play("Idle-Walk-Run");
            }
        }

        protected bool IsPathComplete()
        {
            if (!_navMeshAgent.pathPending)
            {
                if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                {
                    if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }


        private void HandleLookAhead(bool lookAhead)
        {
            if(GetComponent<LookAhead>())
            {
               GetComponent<LookAhead>().enabled = lookAhead; 
            }   
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
