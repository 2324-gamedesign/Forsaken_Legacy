using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ForsakenLegacy
{
    public class TargetEnemy : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private InputAction targetAction;
        private InputAction targetScrollAction;

        private Transform currentTarget;
        private Canvas lockOnCanvas;
        [SerializeField] LayerMask targetLayers;
        public float targetScanRadius = 10f;
        private List<Transform> detectedEnemies = new List<Transform>();
        private int currentTargetIndex = -1;

        // [SerializeField] float noticeZone = 10;

        bool enemyLocked;
        Vector3 pos;

        // Start is called before the first frame update
        void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
            targetAction = _playerInput.actions.FindAction("Target");
            targetScrollAction = _playerInput.actions.FindAction("TargetScroll");
            targetAction.performed += OnTargetPerformed;
            targetScrollAction.performed += OnTargetScroll;
        }

        void OnTargetPerformed(InputAction.CallbackContext context)
        {
            if (enemyLocked)
            {
                DisableTarget();
            }
            else
            {
                ScanNearBy();
            }
        }

        void OnTargetScroll(InputAction.CallbackContext context)
        {
            SwitchTargetWithScroll();
        }

        void EnableTarget(Transform target)
        {
            currentTarget = target;
            lockOnCanvas = currentTarget.gameObject.GetComponentInChildren<Canvas>();
            lockOnCanvas.enabled = true;
            enemyLocked = true;
        }

        void DisableTarget()
        {
            if(currentTarget){lockOnCanvas.enabled = false;}
            currentTarget = null;
            enemyLocked = false;
        }

        private void ScanNearBy()
        {
            detectedEnemies.Clear();

            // Detect enemies within the specified radius
            Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, targetScanRadius, targetLayers);

            // Add detected enemies to the list
            foreach (Collider enemyCollider in nearbyEnemies)
            {
                detectedEnemies.Add(enemyCollider.transform);
            }

            // Lock onto the first detected enemy
            if (detectedEnemies.Count > 0)
            {
                EnableTarget(detectedEnemies[0]);
            }
        }

        void SwitchTargetWithScroll()
        {
            // Check if there are any detected enemies
            if (detectedEnemies.Count == 0)
            {
                currentTargetIndex = -1;
                return;
            }

            // Get the scroll input value
            float scrollInput = targetScrollAction.ReadValue<float>();

            // If scrolling up
            if (scrollInput > 0f)
            {
                // Move to the next target in the array
                currentTargetIndex = (currentTargetIndex + 1) % detectedEnemies.Count;
            }
            // If scrolling down
            else if (scrollInput < 0f)
            {
                // Move to the previous target in the array
                currentTargetIndex = (currentTargetIndex - 1 + detectedEnemies.Count) % detectedEnemies.Count;
            }

            // Lock onto the current target
            LockOntoTarget();
        }

        void LockOntoTarget()
        {
            // Check if there's a valid target index
            if (currentTargetIndex >= 0 && currentTargetIndex < detectedEnemies.Count)
            {
                // Get the current target's transform
                Transform newTarget = detectedEnemies[currentTargetIndex];

                // Enable target lock
                EnableTarget(newTarget);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize the target scan radius in the editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, targetScanRadius);
        }
    }
}
