using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;


namespace ForsakenLegacy
{
    public class Guardian : MonoBehaviour
    {
        public GameObject _target = null;
        public Collider attackRange;

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;

        //floats to handle speed and animator
        private float idleSpeed = 0f;
        private float walkSpeed = 2f;
        private float runSpeed = 4f;

        private float minDistanceToPlayer = 0.9f;
        private bool isInsideAttackRange = false;

        public Collider weaponCollider;
        private bool isAttacking = false;

        //Patrolling
        private Vector3 originalPosition;
        public Transform[] waypoints;
        private Vector3[] path;
        public PathType pathType;
        public float duration;
        public bool isPatrolling;

        public bool isStunned = false;

        public MMFeedbacks feedbackAttack;
        public MMFeedbacks feedbackStun;

        private void Awake() 
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            originalPosition = transform.position;
            
            Patrol();
        }

        // Update is called once per frame
        void Update()
        {
            _animator.SetFloat("Speed", _navMeshAgent.speed);

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

            //If the enemy is active and the parent arena is not in progress and it is not already patrolling, handle the patroll behavior
            if(!GetComponentInParent<Arena>().isInProgress && !isPatrolling)
            {
                Patrol();
            }
        }

        public void StartPursuit()
        {
            isPatrolling = false;
            DOTween.KillAll();

            if(!isStunned)
            {
                //if it is still attacking wait for attack animation to stop and then call this again
                if(isAttacking)
                {
                    Invoke("StartPursuit", 1f);
                    return;
                }
                HandleLookAhead(false);

                // Start pursuing the player
                InvokeRepeating("SetDestination", 0.1f, 0.5f);

                _navMeshAgent.isStopped = false;
                _navMeshAgent.speed = runSpeed;
            }
        }

        private void StopPursuit()
        {
            _navMeshAgent.ResetPath();
            _navMeshAgent.isStopped = true;
        }

        private void SetDestination()
        {
            _navMeshAgent.SetDestination(_target.transform.position);
        }

        private void OnTriggerEnter(Collider other) 
        {
            if(attackRange)
            {
                if(other == attackRange && !_target.GetComponent<HealthSystem>().isDead)
                {
                    CancelInvoke();

                    HandleLookAhead(false);
                    DOTween.KillAll();

                    StopPursuit();

                    InvokeRepeating("TriggerAttack", 0.1f, 5f);
                }
            }
            else return;
        }

        private void OnTriggerExit(Collider other) 
        {
            if(other == attackRange)
            {
                isInsideAttackRange = false;

                CancelInvoke();
                StartPursuit();
            }
        }

        void TriggerAttack()
        {
            if(!_target.GetComponent<HealthSystem>().isDead && !isStunned)
            {
                int indexAttack;
                indexAttack = Random.Range(0, 2);

                feedbackAttack.PlayFeedbacks();

                // Trigger the attack animation or perform attack logic here
                if(indexAttack == 0) _animator.SetTrigger("Attack");
                else if(indexAttack == 1) _animator.SetTrigger("Combo");
            }
            else
            {
                CancelInvoke();
                _navMeshAgent.speed = idleSpeed;

                StopPursuit();
            }
        }

        //Method that handles the start of patrolling
        private void Patrol()
        {  
            isPatrolling = true;
            if(transform.position != originalPosition)
            {
                ReturnToOriginalPosition();
                return;
            }
            //Initialize path
            path = new Vector3[waypoints.Length];
            for (int i = 0; i < waypoints.Length; i++) 
            {
                path[i] = waypoints[i].position;
            }

            HandleLookAhead(true);

            // _navMeshAgent.isStopped = true;
            _navMeshAgent.speed = walkSpeed;
            transform.DOPath(path, duration, pathType, PathMode.Ignore, 10, Color.red).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).SetOptions(closePath: true);

        }

        void ReturnToOriginalPosition()
        {
            DOTween.KillAll();

            _navMeshAgent.SetDestination(originalPosition);
            _navMeshAgent.speed = walkSpeed;
            Patrol();
        }

        private void HandleLookAhead(bool lookAhead)
        {
            if(GetComponent<LookAhead>())
            {
               GetComponent<LookAhead>().enabled = lookAhead; 
            }   
        }

        public void Stun(float duration)
        {
            // Implement your stun logic here, for example, disabling enemy movement
            StartCoroutine(StunCoroutine(duration));
        }
    
        IEnumerator StunCoroutine(float duration)
        {
            isStunned = true;
            StopPursuit();
            _animator.SetTrigger("Stun");
            feedbackStun.PlayFeedbacks();
    
            yield return new WaitForSeconds(duration);

            StunEnd();
        }

        public void StunEnd()
        {
            _animator.SetTrigger("StunEnd");
            feedbackStun.StopFeedbacks();
            isStunned = false;
            StartPursuit();
        }

        //Methods called in animation
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

