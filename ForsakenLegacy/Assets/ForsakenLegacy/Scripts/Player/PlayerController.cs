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
        private float moveSpeed = 2.0f;
        public bool isInAbility = false;
        private float walkSpeed = 2.0f;
        private float sprintSpeed = 5.5f;

        //Footsteps and Gravity
        private Vector3 gravity = new Vector3(0, -20f, 0);
        public float maxSlopeAngle = 60f;
        private float minSlopeAngle = 0.001f;
        private float groundDist = 0.01f;
        public LayerMask groundLayer;
        // private bool isFalling;
        // private float FallTimeout = 0.15f;
        // private float GroundedRadius = 0.28f;
        // private Transform groundCheck;
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
        
        //Camera
        public Transform mainCamera;

        // Player
        private float speedZ;
        private float speedX;
        private float acceleration = 5f;
        private float deceleration = 5f;
        private float maxWalkSpeed = 0.5f;
        private float maxSprintSpeed = 2.0f;
        public float rotateSpeed = 90f;
        public int maxBounces = 5;
        public float anglePower = 0.5f;
        private float playerY;

        //Player Components
        private PlayerInput _playerInput;
        public Animator _animator;
        private Rigidbody _rb;
        // private CharacterController _controller;
        private InputController _input;
        private CapsuleCollider _capsuleCollider;

        //new vars
        public float maxWalkingAngle = 60f;
        private Vector3 velocity;

        //Animations Hash
        private int SpeedZHash;
        private int SpeedXHash;
        private int FallHash;

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
            // _controller = GetComponent<CharacterController>();
            _input = GetComponent<InputController>();
            _rb = GetComponent<Rigidbody>();
            _playerInput = GetComponent<PlayerInput>();
            _capsuleCollider = GetComponent<CapsuleCollider>();

            _rb.isKinematic = true;
            AssignAnimationHash();
        }

        private void OnDisable()
        {
            playerY = transform.position.y;
        }
        private void OnEnable()
        {

            transform.position = new Vector3(transform.position.x, playerY, transform.position.z);
            Debug.Log("Reset Y");
        }


        private void AssignAnimationHash()
        {
            SpeedZHash = Animator.StringToHash("SpeedZ");
            SpeedXHash = Animator.StringToHash("SpeedX");
            FallHash = Animator.StringToHash("Fall");
        }

        private void FixedUpdate()
        {
            bool isAttacking = gameObject.GetComponent<AttackMelee>().isAttacking;
            _hasAnimator = TryGetComponent(out _animator);

            if (canMove && !isInAbility && !isAttacking) {MoveInput();}
        }
        
        private void MoveInput()
        {
            // Get movement input from InputController
            Vector2 moveInput = _input.move;
            bool sprint = _input.sprint;

            bool falling = !isGrounded(out RaycastHit groundHit);
            // If falling, increase falling speed, otherwise stop falling.
            if (falling)
            {
                velocity += Physics.gravity * Time.deltaTime;
            }
            else
            {
                velocity = Vector3.zero;
            }

            // Calculate movement direction based on camera orientation
            Vector3 moveDirection = Vector3.forward * moveInput.y + Vector3.right * moveInput.x;
            moveDirection.y = 0f; // Ensure movement is only in the horizontal plane
            moveDirection.Normalize(); // Normalize the movement direction to ensure    consistent speed in all directions

            Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

            // If the player is standing on the ground, project their movement onto that plane
            // This allows for walking down slopes smoothly.
            if (!falling)
            {
                movement = Vector3.ProjectOnPlane(movement, groundHit.normal);
            }

            // Attempt to move the player based on player movement
            transform.position = MovePlayer(movement);

            // Move player based on falling speed
            transform.position = MovePlayer(velocity * Time.deltaTime);

            //Set current maxSpeed
            float currentMaxSpeed = sprint ? maxSprintSpeed : maxWalkSpeed;

            if (sprint)
            {
                moveSpeed = sprintSpeed;
            }
            else
            {
                moveSpeed = walkSpeed;
            }
            
            // Rotate towards the movement direction
            if (moveDirection.magnitude >= 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                // Reduce rotation speed for smoother rotation
                float rotationSpeed = 5f;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // handle change in speed for animations
            changeSpeed(moveInput, sprint, currentMaxSpeed);
            lockOrResetSpeed(moveInput, sprint, currentMaxSpeed);

            //set animation parameters
            if (_hasAnimator)
            {
               _animator.SetFloat(SpeedXHash, speedX);
               _animator.SetFloat(SpeedZHash, speedZ); 
               _animator.SetBool(FallHash, false);
            }
        }

        //handles acceleration and decelaration
        private void changeSpeed(Vector2 moveInput, bool sprint, float currentMaxSpeed)
        {
            // Increase speed based on movement input
            if (moveInput.y > 0 && speedZ < currentMaxSpeed)
            {
                speedZ += acceleration * Time.deltaTime;
            }
            if (moveInput.y < 0 && speedZ > -currentMaxSpeed)
            {
                speedZ -= acceleration * Time.deltaTime;
            }
            if (moveInput.x < 0 && speedX > -currentMaxSpeed)
            {
                speedX -= acceleration * Time.deltaTime;
            }
            if (moveInput.x > 0 && speedX < currentMaxSpeed)
            {
                speedX += acceleration * Time.deltaTime;
            }

            //decrease speed
            if (moveInput.y == 0 && speedZ > 0f)
            {
                speedZ -= deceleration * Time.deltaTime;
            }

            // increase speed X if left not pressed
            if (moveInput.x >= 0 && speedX < 0f)
            {
                speedX += deceleration * Time.deltaTime;
            }

            //decrease speed X if right not pressed
            if (moveInput.x <= 0 && speedX > 0f)
            {
                speedX -= deceleration * Time.deltaTime;
            }
        }

        //handles reset and lock of speed
        private void lockOrResetSpeed(Vector2 moveInput, bool sprint, float currentMaxSpeed)
        {
            //reset velocity
            if (moveInput.y == 0 && speedZ < 0f)
            {
                speedZ = 0f;
            }

            //reset speedx
            if(moveInput.x == 0 && speedX != 0f && (speedX < 0.05f && speedX > -0.05f))
            {
                speedX = 0f;
            }

            //lockForward
            if (moveInput.y > 0 && sprint && speedZ > currentMaxSpeed)
            {
                speedZ = currentMaxSpeed;
            }
            //decelerate to max walk velocity
            else if (moveInput.y > 0 && speedZ > currentMaxSpeed)
            {
                speedZ -= deceleration * Time.deltaTime;
                //round to currentMaxSpeed if within offset
                if (speedZ > currentMaxSpeed && speedZ < (currentMaxSpeed + 0.05f))
                {
                    speedZ = currentMaxSpeed;
                }
            }
            // round to the currentMaxSpeed if within offset
            else if (moveInput.y > 0 && speedZ < currentMaxSpeed && speedZ > (currentMaxSpeed - 0.05f))
            {
                speedZ = currentMaxSpeed;
            }

            //lock Backward
            if (moveInput.y < 0 && sprint && speedZ < -currentMaxSpeed)
            {
                speedZ = -currentMaxSpeed;
            }
            //decelerate to max walk velocity
            else if (moveInput.y < 0 && speedZ < -currentMaxSpeed)
            {
                speedZ += deceleration * Time.deltaTime;
                //round to currentMaxSpeed if within offset
                if (speedZ < -currentMaxSpeed && speedZ > (-currentMaxSpeed - 0.05f))
                {
                    speedZ = -currentMaxSpeed;
                }
            }
            // round to the currentMaxSpeed if within offset
            else if (moveInput.y < 0 && speedZ > -currentMaxSpeed && speedZ < (-currentMaxSpeed + 0.05f))
            {
                speedZ = -currentMaxSpeed;
            }

            //lock left
            if (moveInput.x < 0 && sprint && speedX < -currentMaxSpeed)
            {
                speedX = -currentMaxSpeed;
            }
            //decelerate to max walk velocity
            else if (moveInput.x < 0 && speedX < -currentMaxSpeed)
            {
                speedX += deceleration * Time.deltaTime;
                //round to currentMaxSpeed if within offset
                if (speedX < -currentMaxSpeed && speedX > (-currentMaxSpeed - 0.05f))
                {
                    speedX = -currentMaxSpeed;
                }
            }
            // round to the currentMaxSpeed if within offset
            else if (moveInput.x < 0 && speedX > -currentMaxSpeed && speedX < (-currentMaxSpeed + 0.05f))
            {
                speedX = -currentMaxSpeed;
            }

            //lock right
            if (moveInput.x > 0 && sprint && speedX > currentMaxSpeed)
            {
                speedX = currentMaxSpeed;
            }
            //decelerate to max walk velocity
            else if (moveInput.x > 0 && speedX > currentMaxSpeed)
            {
                speedX -= deceleration * Time.deltaTime;
                //round to currentMaxSpeed if within offset
                if (speedX > currentMaxSpeed && speedX < (currentMaxSpeed + 0.05f))
                {
                    speedX = currentMaxSpeed;
                }
            }
            // round to the currentMaxSpeed if within offset
            else if (moveInput.x > 0 && speedX < currentMaxSpeed && speedX > (currentMaxSpeed - 0.05f))
            {
                speedX = currentMaxSpeed;
            }
        }

        public Vector3 MovePlayer(Vector3 movement)
        {
            Vector3 position = transform.position;

            Vector3 remaining = movement;

            int bounces = 0;

            while (bounces < maxBounces && remaining.magnitude > minSlopeAngle)
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
                position += hit.normal * minSlopeAngle * 2;
                // Decrease remaining movement by fraction of movement remaining
                remaining *= (1 - fraction);

                // Plane to project rest of movement onto
                Vector3 planeNormal = hit.normal;

                // Only apply angular change if hitting something
                // Get angle between surface normal and remaining movement
                float angleBetween = Vector3.Angle(hit.normal, remaining) - 90.0f;

                // Normalize angle between to be between 0 and 1
                // 0 means no angle, 1 means 90 degree angle
                angleBetween = Mathf.Min(maxSlopeAngle, Mathf.Abs(angleBetween));
                float normalizedAngle = angleBetween / maxSlopeAngle;

                // Reduce the remaining movement by the remaining movement that ocurred
                remaining *= Mathf.Pow(1 - normalizedAngle, anglePower) * 0.9f + 0.1f;

                // Rotate the remaining movement to be projected along the plane of the surface hit (emulate pushing against the object)
                Vector3 projected = Vector3.ProjectOnPlane(remaining, planeNormal).normalized * remaining.magnitude;

                // If projected remaining movement is less than original remaining movement (so if the projection broke due to float operations), then change this to just project along the vertical.
                if (projected.magnitude + minSlopeAngle < remaining.magnitude)
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


        private bool isGrounded(out RaycastHit groundHit)
        {
            bool onGround = CastSelf(transform.position, transform.rotation, Vector3.down, groundDist, out groundHit);
            float angle = Vector3.Angle(groundHit.normal, Vector3.up);
            return onGround && angle < maxWalkingAngle;
        }

        // private void HandleGravity()
        // {
        //     if (Grounded)
        //     {
        //         // reset the fall timeout timer
        //         _fallTimeoutDelta = FallTimeout;

        //         // update animator if using character
        //         if (_hasAnimator)
        //         {
        //             _animator.SetBool(_animIDFall, false);
        //         }

        //         // stop our velocity dropping infinitely when grounded
        //         if (_verticalVelocity < 0.0f)
        //         {
        //             _verticalVelocity = -2f;
        //         }

        //         //if we are still playing the fall end animation don't move
        //         if (_animator.GetCurrentAnimatorStateInfo(0).IsName("FallEnd"))
        //         {
        //             canMove = false;
        //         }
        //         else
        //         {
        //             canMove = true;
		// 		}
        //     }
        //     else
        //     {
        //         // fall timeout
        //         if (_fallTimeoutDelta >= 0.0f)
        //         {
        //             _fallTimeoutDelta -= Time.deltaTime;
        //         }
        //         else
        //         {
        //             // update animator if using character
        //             if (_hasAnimator)
        //             {
        //                 _animator.SetBool(_animIDFall, true);
        //             }
        //         }

        //         // fall timeout
        //         if (_fallTimeoutDelta >= 0.0f)
        //         {
        //             _fallTimeoutDelta -= Time.deltaTime;
        //         }
        //     }

        //     // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        //     if (_verticalVelocity < _terminalVelocity)
        //     {
        //         _verticalVelocity += gravity * Time.deltaTime;
        //     }
        // }


        //Events called in animation
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
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
