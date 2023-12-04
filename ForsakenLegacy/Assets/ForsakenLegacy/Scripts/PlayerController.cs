using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace ForsakenLegacy
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerController : MonoBehaviour
    {
        [Header("Player")]

        public bool canMove = true;
        public float MoveSpeed = 2.0f;

        public float SprintSpeed = 5.335f;

        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        
        public float SpeedChangeRate = 10.0f;

        // Attack
        public bool isAttacking;
        private float maxComboDelay = 1;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
   
        public float Gravity = -15.0f;

        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;
        
        //Camera
        public Camera _mainCamera;

        // Player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _fallTimeoutDelta;

        // Animation Parameters
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDMotionSpeed;
        private int _animIDCombo1;
        private int _animIDCombo2;
        private int _animIDCombo3;


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        public Animator _animator;
        private CharacterController _controller;
        private InputController _input;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void Start()
        { 
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<InputController>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#endif
            AssignAnimationIDs();
        
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            HandleGravity();
            GroundedCheck();
            if (canMove == true) {Move();}
    			    
            Attack();
            HandleAttackAnim();
            SetRootMotion();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDCombo1 = Animator.StringToHash("Combo1");
            _animIDCombo2 = Animator.StringToHash("Combo2");  
            _animIDCombo3 = Animator.StringToHash("Combo3");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void HandleGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }
            else
            {
                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void HandleAttackAnim(){
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Slash1"))
            {
                _animator.SetBool(_animIDCombo1, false);
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Slash2"))
            {
                _animator.SetBool(_animIDCombo2, false);
            }
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Slash3"))
            {
                _animator.SetBool(_animIDCombo3, false);
                _input.noOfClicks = 0;
            }
    
            if (_input.noOfClicks == 0){
                _animator.SetBool(_animIDCombo1, false);
                _animator.SetBool(_animIDCombo2, false);
                _animator.SetBool(_animIDCombo3, false);
            }
            if (Time.time - _input.lastClickedTime > maxComboDelay)
            {
                _input.noOfClicks = 0;
            }
        }
        
        private void Attack() {
            _input.noOfClicks = Mathf.Clamp(_input.noOfClicks, 0, 3);

            if (_input.noOfClicks == 1 && _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle-Walk-Run"))
            {
                _animator.SetBool(_animIDCombo1, true);
            }
            if (_input.noOfClicks >= 2 && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Slash1"))
            {
                _animator.SetBool(_animIDCombo1, false);
                _animator.SetBool(_animIDCombo2, true);
            }
            if (_input.noOfClicks >= 3 && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && _animator.GetCurrentAnimatorStateInfo(0).IsName("Slash2"))
            {
                _animator.SetBool(_animIDCombo2, false);
                _animator.SetBool(_animIDCombo3, true);
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
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}
