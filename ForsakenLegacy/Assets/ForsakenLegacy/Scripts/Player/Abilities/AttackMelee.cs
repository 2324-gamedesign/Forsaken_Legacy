using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using ForsakenLegacy;
using UnityEngine.Animations.Rigging;

namespace ForsakenLegacy
{
    public class AttackMelee : MonoBehaviour
    {
        private InputAction attackAction;
        private PlayerInput _playerInput;

        [SerializeField] private int noOfClicks = 0;
        private float lastClickedTime = 0;

        public bool isAttacking;
        private bool isAttackingCheck = true;
        private float maxComboDelay = 1f;
        public GameObject weapon;
        public AudioClip[] FootstepAudioClips;
        public Rig rigLayer;
        
        public Animator _animator;

        // Feedbacks
        public MMFeedbacks activateWeapon;
        public MMFeedbacks attack;

        private void Start()
        { 
            _playerInput = GetComponent<PlayerInput>();
            rigLayer = GetComponentInChildren<Rig>();

            attackAction = _playerInput.actions.FindAction("Attack");
            attackAction.performed += OnAttackPerformed;
        }

        private void Update()
        {
            noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);
            _animator.SetInteger("noOfClicks", noOfClicks);

            HandleAttackAnim();
            SetRootMotion();
        }

        void OnAttackPerformed(InputAction.CallbackContext context)
        {
            noOfClicks ++;
            lastClickedTime = Time.time;
        }

        private void HandleAttackAnim()
        {
            if (Time.time - lastClickedTime > maxComboDelay)
            {
                noOfClicks = 0;
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Combo3"))
            {
                noOfClicks = 0;
                rigLayer.weight = 0f;
            }

            if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Combo1"))
            {
                rigLayer.weight = 1f;

            }

            // //if the combo3 animation is playing and it is at 20% of its length set the rig weight to 0
            // if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Combo3") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f)
            // {
            //     rigLayer.weight = 0f;
            // }

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Combo1") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo2") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo3") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo1-End") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo2-End") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Stun"))
            {
                isAttacking = true;
                HandleWeapon();
            }
            else
            {
                rigLayer.weight = 0f;
                isAttacking = false;
                HandleWeapon();
            }
        }

        private void HandleWeapon()
        {
            if (isAttackingCheck != isAttacking)
            {
                if(isAttacking)
                {
                    GameManager.Instance.SetAttackState();
                }
                else
                {
                    GameManager.Instance.SetMoveState();
                }
                weapon.gameObject.SetActive(isAttacking);
                activateWeapon.PlayFeedbacks();
                attack.PlayFeedbacks();
                isAttackingCheck = isAttacking;
            }
        }

        private void SetRootMotion()
        {
            if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle-Walk-Run") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Dash")){
                _animator.applyRootMotion = false;
                GetComponent<PlayerController>().enabled = true; // Enable the PlayerController when not in attack animation
            }
            else
            {
                _animator.applyRootMotion = true;
                GetComponent<PlayerController>().enabled = false; // Disable the PlayerController during attack animations
            }
        }

        // Methods called in animation
        private void WeaponColliderOn()
        {
            weapon.GetComponent<Collider>().enabled = true;
        }
        private void WeaponColliderOff()
        {
            weapon.GetComponent<Collider>().enabled = false;
        }
        private void HoldWeapon()
        {
            // rigLayer.weight = 1f;
            weapon.gameObject.SetActive(true);
            activateWeapon.PlayFeedbacks();
        }

        private void ReleaseWeapon()
        {
            // rigLayer.weight = 0f;
            weapon.gameObject.SetActive(false);
            activateWeapon.PlayFeedbacks();
        }
    }
}

