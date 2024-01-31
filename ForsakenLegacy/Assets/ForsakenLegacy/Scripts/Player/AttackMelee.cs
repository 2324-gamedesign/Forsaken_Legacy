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
        public RigLayer rigLayer;
        
        public Animator _animator;
        private CharacterController _controller;

        // Feedbacks
        public MMFeedbacks activateWeapon;
        public MMFeedbacks attack;

        private void Start()
        { 
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();

            attackAction = _playerInput.actions.FindAction("Attack");
            attackAction.performed += OnAttackPerformed;
        }

        private void Update()
        {
            bool isDashing = gameObject.GetComponent<DashAbility>().isDashing;
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
            }


            if(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo3")) {
                rigLayer.rig.weight = 0; 
            }

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Combo1") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo2") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo3") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo1-End") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo2-End"))
            {
                isAttacking = true;
                HandleWeapon();
            }
            else
            {
                isAttacking = false;
                HandleWeapon();
            }
        }

        private void HandleWeapon()
        {
            if (isAttackingCheck != isAttacking)
            {
                isAttackingCheck = isAttacking;
                weapon.gameObject.SetActive(isAttacking);
                activateWeapon.PlayFeedbacks();
                attack.PlayFeedbacks();
                if(isAttacking){
                    rigLayer.rig.weight = 1;
                }
                else{
                    rigLayer.rig.weight = 0; 
                }
            }
        }

        private void SetRootMotion(){
            if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle-Walk-Run")){
                _animator.applyRootMotion = false;
            }
            else
            {
                _animator.applyRootMotion = true;
            }
        }

        // Methods calld in animation
        private void WeaponColliderOn()
        {
            weapon.GetComponent<Collider>().enabled = true;
        }
        private void WeaponColliderOff()
        {
            weapon.GetComponent<Collider>().enabled = false;
        }
    }
}

