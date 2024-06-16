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
        private Transform _target;
        private Collider _attackRange;
        private Vector3 _randomTargetOffset;

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;

        //floats to handle speed and animator
        private readonly float _idleSpeed = 0f;
        private readonly float _walkSpeed = 2f;
        private readonly float _runSpeed = 4f;

        private readonly float _minDistanceToPlayer = 0.9f;
        private bool _isInsideAttackRange = false;

        public Collider WeaponCollider;
        private bool _isAttacking = false;
        public bool _isStunned = false;

        public MMFeedbacks FeedbackAttack;
        public MMFeedbacks FeedbackSpawn;

        private void Awake() 
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();

            //find player by name
            _target = GameObject.Find("Edea").transform;
            _attackRange = GameObject.FindGameObjectWithTag("Player").GetComponent<SphereCollider>();

            _randomTargetOffset = Random.insideUnitCircle * (_attackRange.bounds.extents.x * 0.8f);
        }

        // Update is called once per frame
        void Update()
        {
            _animator.SetFloat("Speed", _navMeshAgent.speed);

            //If it's in attack, avoid enemy penetrating into player
            if (_isAttacking)
            {
                Vector3 toPlayer = _target.transform.position - transform.position;
                float distanceToPlayer = toPlayer.magnitude;

                if (distanceToPlayer < _minDistanceToPlayer)
                {
                    Vector3 newPosition = _target.transform.position - toPlayer.normalized * _minDistanceToPlayer;
                    transform.position = newPosition;
                }
            }
        }

        public void Spawn()
        {
            FeedbackSpawn.PlayFeedbacks();
        }
        public void StartPursuit()
        {
            CancelInvoke(nameof(TriggerAttack));

            if(!GetComponent<Stunnable>().isStunned)
            {
                //if it is still attacking wait for attack animation to stop and then call this again
                if(_isAttacking)
                {
                    Invoke(nameof(StartPursuit), 0.5f);
                    return;
                }

                HandleLookAhead(false);

                // Start pursuing the player
                InvokeRepeating(nameof(SetDestination), 0.1f, 0.5f);

                _navMeshAgent.isStopped = false;
                _navMeshAgent.speed = _runSpeed;
            }
        }

        public void StopPursuit()
        {
            CancelInvoke();
            _navMeshAgent.ResetPath();
            _navMeshAgent.isStopped = true;
        }

        private void SetDestination()
        {
            _navMeshAgent.SetDestination(_target.transform.position + _randomTargetOffset);
        }

        private void OnTriggerEnter(Collider other) 
        {
            if(_attackRange && !_isInsideAttackRange)
            {
                if(other == _attackRange && !_target.GetComponent<HealthSystem>().IsDead)
                {
                    _isInsideAttackRange = true;
                    Debug.Log("Enter Attack Range");

                    CancelInvoke(nameof(SetDestination));

                    HandleLookAhead(false);
                    DOTween.KillAll();

                    StopPursuit();

                    InvokeRepeating(nameof(TriggerAttack), 0.1f, 5f);
                }
            }
            else return;
        }

        private void OnTriggerExit(Collider other) 
        {
            if(other == _attackRange)
            {
                Debug.Log("Exit Attack Range");
                _isInsideAttackRange = false;

                CancelInvoke(nameof(TriggerAttack));
                StartPursuit();
            }
        }

        void TriggerAttack()
        {
            if(!_target.GetComponent<HealthSystem>().IsDead && !GetComponent<Stunnable>().isStunned)
            {
                int indexAttack;
                indexAttack = Random.Range(0, 2);

                FeedbackAttack.PlayFeedbacks();

                // Trigger the attack animation or perform attack logic here
                if(indexAttack == 0) _animator.SetTrigger("Attack");
                else if(indexAttack == 1) _animator.SetTrigger("Combo");
            }
            else
            {
                CancelInvoke(nameof(TriggerAttack));
                _navMeshAgent.speed = _idleSpeed;
            }
        }

        // void ReturnToOriginalPosition()
        // {
        //     DOTween.KillAll();

        //     _navMeshAgent.SetDestination(originalPosition);
        //     _navMeshAgent.speed = walkSpeed;
        //     Patrol();
        // }

        private void HandleLookAhead(bool lookAhead)
        {
            if(GetComponent<LookAhead>())
            {
               GetComponent<LookAhead>().enabled = lookAhead;
            }   
        }

        // public void Stun(float duration)
        // {
        //     // Implement your stun logic here, for example, disabling enemy movement
        //     StartCoroutine(StunCoroutine(duration));
        // }
    
        // IEnumerator StunCoroutine(float duration)
        // {
        //     isStunned = true;
        //     StopPursuit();
        //     _animator.SetTrigger("Stun");
        //     feedbackStun.PlayFeedbacks();
    
        //     yield return new WaitForSeconds(duration);

        //     StunEnd();
        // }

        // public void StunEnd()
        // {
        //     _animator.SetTrigger("StunEnd");
        //     feedbackStun.StopFeedbacks();
        //     isStunned = false;
        //     StartPursuit();
        // }

        //Methods called in animation
        void SetStop()
        {
            _isAttacking = true;
            _navMeshAgent.speed = _idleSpeed;
        }
        void SetGo()
        {
            if(!_isInsideAttackRange)
            {
                _isAttacking = false;
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
            WeaponCollider.enabled = true;
        }
        void ColliderWeaponOff()
        {
            WeaponCollider.enabled = false;
        }
    }
}

