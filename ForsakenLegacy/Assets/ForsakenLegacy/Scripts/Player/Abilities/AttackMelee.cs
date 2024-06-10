using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using ForsakenLegacy;
using UnityEngine.Animations.Rigging;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

namespace ForsakenLegacy
{
    public class AttackMelee : MonoBehaviour
    {
        private InputAction attackAction;
        private PlayerInput _playerInput;
        private PlayerController _playerController;
        [SerializeField] private int noOfClicks = 0;
        private float lastClickedTime = 0;

        public bool isAttacking;
        private bool isAttackingCheck = true;
        private float maxComboDelay = 1f;
        public GameObject weapon;
        public Rig rigLayer;
        public Animator _animator;

        //Audio
        private AudioSource _audioSource;
        public AudioClip[] SlashAudioClips;

        // Feedbacks
        public MMFeedbacks activateWeapon;
        public MMFeedbacks attack;

        private void Start()
        { 
            _playerInput = GetComponent<PlayerInput>();
            rigLayer = GetComponentInChildren<Rig>();
            _playerController = GetComponent<PlayerController>();

            attackAction = _playerInput.actions.FindAction("Attack");
            attackAction.performed += OnAttackPerformed;
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);
            _animator.SetInteger("noOfClicks", noOfClicks);

            HandleAttackAnim();
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

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Combo1") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo2") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo3") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo1-End") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Combo2-End") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Stun"))
            {
                isAttacking = true;
                _animator.ResetTrigger("isStopping");
                HandleWeapon();
            }
            else
            {
                rigLayer.weight = 0f;
                isAttacking = false;
                HandleWeapon();
                if(noOfClicks > 1)
                {
                    noOfClicks = 0;
                }
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
                    WeaponColliderOff();
                    noOfClicks = 0;
                }
                // weapon.gameObject.SetActive(isAttacking);
                // activateWeapon.PlayFeedbacks();
                attack.PlayFeedbacks();
                isAttackingCheck = isAttacking;
            }
        }

        private void OnAnimatorMove() 
        {
            if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle-Walk-Run") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Dash") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Stopping") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                _animator.applyRootMotion = false;
                return;
            }

            Vector3 movement = _animator.deltaPosition;
            if (!_playerController.CastSelf(transform.position, transform.rotation, movement.normalized, movement.magnitude, out RaycastHit hit))
            {
                _animator.ApplyBuiltinRootMotion();
            }
            else
            {
                _animator.applyRootMotion = false;
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
        private void OnSlash(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (SlashAudioClips.Length > 0)
                {
                    var index = Random.Range(0, SlashAudioClips.Length);
                    _audioSource.clip = SlashAudioClips[index];
                    // Play an attack sound
                    _audioSource.Play();
                }
            }
        }
    }
}

