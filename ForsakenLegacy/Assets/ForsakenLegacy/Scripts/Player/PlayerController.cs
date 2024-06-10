using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace ForsakenLegacy
{
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerController : MonoBehaviour, IDataPersistence
    {
        [Header("Player")]

        [SerializeField] private bool canMove = true;
        private float _moveSpeed = 2.0f;
        public bool _isInAbility = false;
        private bool _movementPressed;
        public float _maxWalkingAngle = 60f;
        private float _speed;
        private Vector3 _velocity;
        private bool _isStopping;

        //Footsteps and Gravity
        public float MaxSlopeAngle = 60f;
        private readonly float _minSlopeAngle = 0.001f;
        private readonly float _groundDist = 0.1f;
        public LayerMask GroundLayer;
 
        [Header("Audio")]
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        private AudioSource _audioSource;

        [Range(0, 1)] public float FootstepAudioVolume = 0.2f;
        
        //Camera
        [Header("Camera")]
        public Transform MainCamera;

        // Player Readonly Variables
        private readonly float _walkSpeed = 2.0f;
        private readonly float _sprintSpeed = 7f;
        private readonly float _acceleration = 3f;
        private readonly float _maxWalkSpeed = 0.5f;
        private readonly float _maxSprintSpeed = 2.0f;
        private readonly float _rotateSpeed = 90f;
        private readonly int _maxBounces = 5;
        private readonly float _anglePower = 0.5f;
        // private float playerY;

        //Player Components
        private PlayerInput _playerInput;
        private Animator _animator;
        private Rigidbody _rb;
        private InputController _input;
        private CapsuleCollider _capsuleCollider;


        //Animations Hash
        private readonly int _speedHash = Animator.StringToHash("Speed");
        private readonly int _fallHash = Animator.StringToHash("Fall");
        private readonly int _isStoppingHash = Animator.StringToHash("isStopping");

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

        public void LoadData(GameData data)
        {
            this.transform.position = data.playerPosition;
        }
        public void SaveData(ref GameData data)
        {
            data.playerPosition = this.transform.position;
        }

        private void Start()
        { 
            _hasAnimator = TryGetComponent(out _animator);

            _input = GetComponent<InputController>();
            _rb = GetComponent<Rigidbody>();
            _playerInput = GetComponent<PlayerInput>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _audioSource = GetComponent<AudioSource>();
            
            _rb.isKinematic = true;
        }

        private void FixedUpdate()
        {
            bool isAttacking = gameObject.GetComponent<AttackMelee>().isAttacking;
            _hasAnimator = TryGetComponent(out _animator);

            if (canMove && !_isInAbility && !isAttacking) {MoveInput();}

            PushOutIfPenetrating();
        }

        private void PushOutIfPenetrating()
        {
            // //Check for overlapping colliders
            // Collider[] colliders = Physics.OverlapCapsule(
            //     transform.position,
            //     transform.position + Vector3.up * (0.25f + _capsuleCollider.height),
            //     _capsuleCollider.radius,
            //     GroundLayer);
            // //Loop through the detected colliders
            // foreach (Collider collider in colliders)
            // {
            //     if (collider != _capsuleCollider)
            //     {
            //         // Calculate the direction and distance to push the player out
            //         Vector3 direction = transform.position - collider.ClosestPoint(transform.position);
            //         float distance = _capsuleCollider.radius - direction.magnitude;
            //         // Apply the push out
            //         transform.position += direction.normalized * distance;
            //         Debug.Log("pushing out" + distance);
            //     }
            // }
            // if (Physics.CapsuleCast(transform.position, transform.position + Vector3.up * (0.25f + _capsuleCollider.height), _capsuleCollider.radius, transform.position + Vector3.down, out RaycastHit hits))
            // {
            //     Debug.Log("hitting");
            // }
           
            //Check if the player is too close to the ground and adjust position upwards if necessary
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 1, GroundLayer))
            {
                transform.position += Vector3.up * (1 - hit.distance);
                Debug.Log("pushing out");
            }


        }

        private void MoveInput()
        {
            // Get movement input from InputController
            Vector2 moveInput = _input.move;
            bool sprint = _input.sprint;

            bool falling = !IsGrounded(out RaycastHit groundHit);
            // If falling, increase falling speed, otherwise stop falling.
            if (falling)
            {
                _velocity += Physics.gravity * Time.deltaTime;
            }
            else
            {
                _velocity = Vector3.zero;
            }

            // Calculate movement direction based on camera orientation
            Vector3 moveDirection = Vector3.forward * moveInput.y + Vector3.right * moveInput.x;
            moveDirection.y = 0f; // Ensure movement is only in the horizontal plane
            moveDirection.Normalize(); // Normalize the movement direction to ensure    consistent speed in all directions

            Vector3 movement = moveDirection * _moveSpeed * Time.deltaTime;

            // If the player is standing on the ground, project their movement onto that plane
            // This allows for walking down slopes smoothly.
            if (!falling)
            {
                movement = Vector3.ProjectOnPlane(movement, groundHit.normal);
            }

            // Attempt to move the player based on player movement
            transform.position = MovePlayer(movement);

            // Move player based on falling speed
            transform.position = MovePlayer(_velocity * Time.deltaTime);

            //Set current maxSpeed
            float currentMaxSpeed = sprint ? _maxSprintSpeed : _maxWalkSpeed;

            // Update moveSpeed based on sprinting
            _moveSpeed = sprint ? _sprintSpeed : _walkSpeed;
            
            // Rotate towards the movement direction
            if (moveDirection.magnitude >= 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                // Reduce rotation speed for smoother rotation
                float rotationSpeed = 5f;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // handle change in speed for animations
            ChangeSpeedAnimation(moveInput, currentMaxSpeed);

            // Detect sudden stop and set isStopping flag
            DetectSuddenStop();

            //set animation parameters
            if (_hasAnimator)
            {
               _animator.SetFloat(_speedHash, _speed);
               _animator.SetBool(_fallHash, false);
            }
        }

        // Handles acceleration and deceleration
        private void ChangeSpeedAnimation(Vector2 moveInput, float currentMaxSpeed)
        {
            float targetSpeed;

            // Decrease speed when there is no input
            if (moveInput == Vector2.zero)
            {
                targetSpeed = 0f;
                if(_movementPressed)
                {
                    _movementPressed = false;
                    _animator.SetTrigger(_isStoppingHash);
                }
            }
            else
            {
                targetSpeed = currentMaxSpeed;
                _movementPressed = true;
                _animator.ResetTrigger(_isStoppingHash);
            }
            
            _speed = Mathf.MoveTowards(_speed, targetSpeed, _acceleration * Time.deltaTime);
        }

        // Detects sudden stop and sets the isStopping flag
        private void DetectSuddenStop()
        {
            float previousSpeed = _speed;
            bool wasRunning = previousSpeed > 1.5f;
            bool isNowIdle = Mathf.Approximately(_speed, 0f);

            if (wasRunning && isNowIdle)
            {
                _isStopping = true;
            }
            else
            {
                _isStopping = false;
            }
        }

        public Vector3 MovePlayer(Vector3 movement)
        {
            Vector3 position = transform.position;

            Vector3 remaining = movement;

            int bounces = 0;

            while (bounces < _maxBounces && remaining.magnitude > _minSlopeAngle)
            {
                // Do a cast of the collider to see if an object is hit during this movement bounce
                float distance = remaining.magnitude;
                if (!CastSelf(position, transform.rotation, remaining.normalized, distance, out RaycastHit hit))
                {
                    // If there is no hit, move to desired position
                    position += remaining;

                    // Exit as we are done bouncing
                    break;
                }

                // If we are overlapping with something, just exit.
                if (hit.distance == 0)
                {
                    break;
                }

                float fraction = hit.distance / distance;
                // Set the fraction of remaining movement (minus some small value)
                position += remaining * (fraction);
                // Push slightly along normal to stop from getting caught in walls
                position += hit.normal * _minSlopeAngle * 2;
                // Decrease remaining movement by fraction of movement remaining
                remaining *= (1 - fraction);

                // Plane to project rest of movement onto
                Vector3 planeNormal = hit.normal;

                // Only apply angular change if hitting something
                // Get angle between surface normal and remaining movement
                float angleBetween = Vector3.Angle(hit.normal, remaining) - 90.0f;

                // Normalize angle between to be between 0 and 1
                // 0 means no angle, 1 means 90 degree angle
                angleBetween = Mathf.Min(MaxSlopeAngle, Mathf.Abs(angleBetween));
                float normalizedAngle = angleBetween / MaxSlopeAngle;

                // Reduce the remaining movement by the remaining movement that ocurred
                remaining *= Mathf.Pow(1 - normalizedAngle, _anglePower) * 0.9f + 0.1f;

                // Rotate the remaining movement to be projected along the plane of the surface hit (emulate pushing against the object)
                Vector3 projected = Vector3.ProjectOnPlane(remaining, planeNormal).normalized * remaining.magnitude;

                // If projected remaining movement is less than original remaining movement (so if the projection broke due to float operations), then change this to just project along the vertical.
                if (projected.magnitude + _minSlopeAngle < remaining.magnitude)
                {
                    remaining = Vector3.ProjectOnPlane(remaining, Vector3.up).normalized * remaining.magnitude;
                }
                else
                {
                    remaining = projected;
                }

                // Track number of times the character has bounced
                bounces++;
            }

            // We're done, player was moved as part of the loop
            return position;
        }

        public bool CastSelf(Vector3 pos, Quaternion rot, Vector3 dir, float dist, out RaycastHit hit)
        {
            // Get Parameters associated with the KCC
            Vector3 center = rot * _capsuleCollider.center + pos;
            float radius = _capsuleCollider.radius;
            float height = _capsuleCollider.height;

            // Get top and bottom points of collider
            Vector3 bottom = center + rot * Vector3.down * (height / 2 - radius);
            Vector3 top = center + rot * Vector3.up * (height / 2 - radius);

            // Check what objects this collider will hit when cast with this configuration excluding itself
            IEnumerable<RaycastHit> hits = Physics.CapsuleCastAll(top, bottom, radius, dir, dist, ~0, QueryTriggerInteraction.Ignore).Where(hit => hit.collider.transform != transform);
            bool didHit = hits.Count() > 0;

            // Find the closest objects hit
            float closestDist = didHit ? Enumerable.Min(hits.Select(hit => hit.distance)) : 0;
            IEnumerable<RaycastHit> closestHit = hits.Where(hit => hit.distance == closestDist);

            // Get the first hit object out of the things the player collides with
            hit = closestHit.FirstOrDefault();

            // Return if any objects were hit
            return didHit;
        }

        private bool IsGrounded(out RaycastHit groundHit)
        {
            bool onGround = CastSelf(transform.position, transform.rotation, Vector3.down, _groundDist, out groundHit);
            float angle = Vector3.Angle(groundHit.normal, Vector3.up);
            return onGround && angle < _maxWalkingAngle;
        }

        //Events called in animation
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    _audioSource.clip = FootstepAudioClips[index];
                    _audioSource.Play();
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.transform.position, FootstepAudioVolume);
            }
        }
    }
}
