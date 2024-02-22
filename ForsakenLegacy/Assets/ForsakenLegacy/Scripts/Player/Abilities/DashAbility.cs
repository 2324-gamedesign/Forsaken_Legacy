using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using ForsakenLegacy;

namespace ForsakenLegacy
{
    public class DashAbility : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private CharacterController _controller;
    
        private InputAction dashAction;
        public float dashDistance = 5.0f;
        private float dashDuration = 0.2f;
        private bool canDash = true;
        private Vector3 dashDirection;
    
        public float dashCooldown = 5f;
        public Image dashCooldownImage; 
    
        public GameObject trail;
        public MMFeedbacks dashParticles;
        private Animator _animator;
    
        // Start is called before the first frame update
        void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
            _controller = GetComponent<CharacterController>();
            dashAction = _playerInput.actions.FindAction("Dash");
            dashAction.performed += OnDashPerformed;
    
            _animator = GetComponent<Animator>();
    
        }
    
        void OnDashPerformed(InputAction.CallbackContext context)
        {
            bool isAttacking = GetComponent<AttackMelee>().isAttacking;
            
            if (!GetComponent<PlayerController>().isInAbility && canDash && !isAttacking)
            {
                StartCoroutine(PerformDash());
            }
            else
            {
                return;
            }
        }
        private IEnumerator PerformDash()
        {
            GetComponent<PlayerController>().isInAbility = true;
            canDash = false;
    
            _animator.Play("Dash");
            // FadeOut();
    
            trail.SetActive(true);
            dashParticles.PlayFeedbacks();
    
            // Store the current position, rotation, and y-coordinate
            Vector3 startPosition = transform.position + Vector3.up * 1f; // Offset the starting position upward
            float startY = transform.position.y;
    
            dashDirection = transform.forward;
            _controller.enabled = false;
    
            // Perform a sphere cast in the dash direction to check if there is any obstacle
            if (Physics.SphereCast(startPosition, _controller.radius, dashDirection, out RaycastHit hit, dashDistance, LayerMask.GetMask("Ground", "Environment")))
            {
                // Adjust the player's position to the point of contact with the obstacle to avoid compenetration
                transform.position = hit.point - dashDirection * _controller.radius;
            }
            else
            {
                // If no obstacle, move the player instantly in the dash direction
                transform.position += dashDirection * dashDistance;
            }
    
            yield return new WaitForSeconds(dashDuration);
    
            transform.position = new Vector3(transform.position.x, startY, transform.position.z);
            _controller.enabled = true;
    
            // FadeIn();
            dashCooldownImage.fillAmount = 1;
    
            GetComponent<PlayerController>().isInAbility = false;
            StartCoroutine(DashCooldown());
    
            yield return new WaitForSeconds(dashDuration);
            trail.SetActive(false);
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
    
        public void FadeOut() {
            Renderer[] renderer = GetComponentsInChildren<Renderer>();
    
            foreach(Renderer i in renderer)
            i.enabled = false;
        }
    
        public void FadeIn() {
            Renderer[] renderer = GetComponentsInChildren<Renderer>();
    
            foreach(Renderer i in renderer)
            i.enabled = true;
        }
    }
}
