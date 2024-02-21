using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.InputSystem;
using UnityEngine;

namespace ForsakenLegacy
{
    public class StunAbility : MonoBehaviour
    {
        public float stunDuration = 3f;
        public float stunRadius = 5f;

        private LayerMask enemyLayer;

        private PlayerInput _playerInput;
        private InputAction stunAction;

        public MMFeedbacks stunFeedback;

        private void Start() 
        {
            //set the layermask to the enemy layer
            enemyLayer = LayerMask.GetMask("Enemy");

            //initialize the input system to check for the key
            _playerInput = GetComponent<PlayerInput>();
            stunAction = _playerInput.actions.FindAction("Stun");
            stunAction.performed += OnStunPerformed;
        }

        private void OnStunPerformed(InputAction.CallbackContext context)
        {
            bool isAttacking = GetComponent<AttackMelee>().isAttacking;
            
            if (!GetComponent<PlayerController>().isInAbility && !isAttacking)
            {
                StartCoroutine("PerformStun");
            }
            else return;
        }

        private IEnumerator PerformStun()
        {
            GetComponent<PlayerController>().isInAbility = true;

            // Play the stun feedback
            stunFeedback.PlayFeedbacks();

            yield return new WaitForSeconds(1);

            // Get all colliders in the stun radius that belong to the enemy layer
            Collider[] colliders = Physics.OverlapSphere(transform.position, stunRadius, enemyLayer);

            // Apply the stun effect to each enemy
            foreach (Collider collider in colliders)
            {
                Stunnable stunnable = collider.GetComponent<Stunnable>();
                if (stunnable != null)
                {
                    stunnable.Stun(stunDuration);
                }
            }

            GetComponent<PlayerController>().isInAbility = false;
        }
    }
}

