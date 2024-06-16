using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using ForsakenLegacy;
using UnityEngine.VFX;

namespace ForsakenLegacy
{
    public class DashAbility : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private CapsuleCollider _capsuleCollider;
    
        private InputAction dashAction;
        public float dashDistance = 5.0f;
        private float dashDuration = 0.2f;
        private bool canDash = true;
        private Vector3 dashDirection;
    
        public float dashCooldown = 5f;
        public Image dashCooldownImage; 
    
        public VisualEffect trail;
        public MMFeedbacks dashFeedback;
        private Animator _animator;
    
        // Start is called before the first frame update
        void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            dashAction = _playerInput.actions.FindAction("Dash");
            dashAction.performed += OnDashPerformed;
    
            _animator = GetComponent<Animator>();
    
        }
    
        void OnDashPerformed(InputAction.CallbackContext context)
        {
            if (this != null)
            {
                bool isAttacking = GetComponent<AttackMelee>().isAttacking;
                
                if (canDash && !isAttacking)
                {
                    StartCoroutine(PerformDash());
                }
                else
                {
                    return;
                }
            }
        }
        private IEnumerator PerformDash()
        {
            GameManager.Instance.SetAbilityState();
            canDash = false;
        
            _animator.Play("Dash");
            dashFeedback.PlayFeedbacks();
        
            trail.Play();
        
            Vector3 dashDirection = transform.forward;
        
            // Calculate the dash destination
            Vector3 dashDestination = transform.position + dashDirection * dashDistance;
        
            // Store the initial position
            Vector3 initialPosition = transform.position;
        
            // Time elapsed
            float elapsedTime = 0f;
        
            // Maximum distance to cover during each iteration
            float maxDistancePerIteration = dashDistance / (dashDuration / Time.deltaTime);
        
            while (elapsedTime < dashDuration)
            {
                // Calculate the current position based on lerping between initial and dash destinations
                Vector3 currentPosition = Vector3.Lerp(initialPosition, dashDestination, elapsedTime / dashDuration);
        
                // Calculate the movement vector
                Vector3 movement = currentPosition - transform.position;
        
                // Perform sphere cast for collisions
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, _capsuleCollider.radius - 0.2f, dashDirection, out hit, maxDistancePerIteration, LayerMask.GetMask("Ground","Environment")))
                {
                    // Calculate the offset position slightly before the point of contact with the obstacle
                    Vector3 newPosition = hit.point - movement.normalized * 0.2f;
        
                    // Set the player's position to the offset position to avoid penetration
                    transform.position = newPosition;
        
                    // Exit the loop if a collision is detected
                    break;
                }
        
                // Use the PlayerController's MovePlayer method to handle slopes and remaining movement
                transform.position = GetComponent<PlayerController>().MovePlayer(movement);
        
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            dashCooldownImage.fillAmount = 1;
            trail.Stop();
        
            GameManager.Instance.SetMoveState();
            StartCoroutine(DashCooldown());
        }
        private IEnumerator DashCooldown()
        {
            float elapsedTime = 0f;
    
            while (elapsedTime < dashCooldown)
            {
                // Calculate the fill amount based on the remaining cooldown time
                float fillAmount = 1 - (elapsedTime / dashCooldown);
                // Update the UI image's fill amount
                dashCooldownImage.fillAmount = fillAmount;
                // Increment the elapsed time
                elapsedTime += Time.deltaTime;
                // Wait for the next frame
                yield return null;
            }
            // Allow dashing again after the cooldown
            canDash = true;
        }
    }
}
