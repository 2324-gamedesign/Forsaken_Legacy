using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

namespace ForsakenLegacy
{
    public class AttackMelee : MonoBehaviour
    {
        public bool isAttacking;
        private bool isAttackingCheck = true;
        private float maxComboDelay = 1;
        public GameObject weapon;
        public AudioClip[] FootstepAudioClips;
        public RigLayer rigLayer;
        
        public Animator _animator;
        private CharacterController _controller;
        private InputController _input;

        // Feedbacks
        public MMFeedbacks activateWeapon;

        private void Start()
        { 
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<InputController>();
        }

        private void Update()
        {
            bool isDashing = gameObject.GetComponent<DashAbility>().isDashing;

            if(_input.noOfClicks > 0 && !isDashing) {Attack();}
            HandleAttackAnim();
            SetRootMotion();
        }

        private void HandleAttackAnim()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo1") || _input.noOfClicks < 1)
            {
                _animator.SetBool("Combo1", false);
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo2") || _input.noOfClicks < 2)
            {
                _animator.SetBool("Combo2", false);
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo3"))
            {
                _animator.SetBool("Combo3", false);
                _input.noOfClicks = 0;
            }
    
            if (_input.noOfClicks == 0){
                _animator.SetBool("Combo1", false);
                _animator.SetBool("Combo2", false);
                _animator.SetBool("Combo3", false);
            }
            if (Time.time - _input.lastClickedTime > maxComboDelay)
            {
                _input.noOfClicks = 0;
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
        
        private void Attack() 
        {
            _input.noOfClicks = Mathf.Clamp(_input.noOfClicks, 0, 3);

            if (_input.noOfClicks == 1 && _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle-Walk-Run"))
            {
                _animator.SetBool("Combo1", true);
            }
            
            if (_input.noOfClicks >= 2 && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo1") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo1-End"))
            {
                _animator.SetBool("Combo1", false);
                _animator.SetBool("Combo2", true);
            }
            
            if (_input.noOfClicks >= 3 && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo2"))
            {
                _animator.SetBool("Combo1", false);
                _animator.SetBool("Combo2", false);
                _animator.SetBool("Combo3", true);
            }else if(_input.noOfClicks >= 3 && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo2-End")){
                _animator.SetBool("Combo1", false);
                _animator.SetBool("Combo2", false);
                _animator.SetBool("Combo3", true);
            }
        }

        private void HandleWeapon()
        {
            if (isAttackingCheck != isAttacking)
            {
                isAttackingCheck = isAttacking;
                weapon.gameObject.SetActive(isAttacking);
                activateWeapon?.PlayFeedbacks();
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

        private void WeaponColliderOn()
        {
            if(weapon)
            {
                weapon.GetComponent<Collider>().enabled = true;
            }
        }
        private void WeaponColliderOff()
        {
            if(weapon)
            {
                weapon.GetComponent<Collider>().enabled = false;
            }
        }
    }
}

