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
        public GameObject lockOnCanvasPrefab; // Assign the lock-on canvas prefab in the Inspector
        private Canvas lockOnCanvasInstance;
        [SerializeField] LayerMask targetLayers;
        public float targetScanRadius = 10f;
        private List<Transform> detectedEnemies = new List<Transform>();
        private int currentTargetIndex = -1;

        bool enemyLocked;

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
            float scrollValue = context.ReadValue<float>();
            SwitchTargetWithScroll(scrollValue);
        }

        void EnableTarget(Transform target)
        {
            currentTarget = target;
            enemyLocked = true;
            RepositionCanvas();
        }

        void DisableTarget()
        {
            if (lockOnCanvasInstance != null)
            {
                Destroy(lockOnCanvasInstance.gameObject);
            }
            detectedEnemies.Clear();
            currentTarget = null;
            enemyLocked = false;
        }

        private void ScanNearBy()
        {
            detectedEnemies.Clear();
        
            // Detect enemies within the specified radius
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, targetScanRadius, targetLayers);
        
            // Filter the colliders to only include those with a CapsuleCollider
            foreach (Collider enemyNearby in nearbyColliders)
            {
                CapsuleCollider capsuleCollider = enemyNearby.GetComponent<CapsuleCollider>();
                if (capsuleCollider != null)
                {
                    detectedEnemies.Add(enemyNearby.transform);
                }
            }
        
            // Lock onto the first detected enemy
            if (detectedEnemies.Count > 0)
            {
                EnableTarget(detectedEnemies[0]);
            }
        }

        void SwitchTargetWithScroll(float scrollInput)
        {
            Debug.Log(scrollInput);
            // Check if there are any detected enemies
            if (detectedEnemies.Count == 0)
            {
                currentTargetIndex = -1;
                DisableTarget();
                return;
            }

            // If scrolling up
            if (scrollInput > 0f)
            {
                // Move to the previous target in the array
                currentTargetIndex = (currentTargetIndex - 1 + detectedEnemies.Count) % detectedEnemies.Count;
            }
            // If scrolling down
            else if (scrollInput < 0f)
            {
                // Move to the next target in the array
                currentTargetIndex = (currentTargetIndex + 1) % detectedEnemies.Count;
            }

            // Lock onto the current target
            LockOntoTarget();
        }

        void LockOntoTarget()
        {
            // Check if there's a valid target index
            if (currentTargetIndex >= 0 && currentTargetIndex < detectedEnemies.Count)
            {
                // Destroy the existing canvas (if any)
                if (lockOnCanvasInstance != null)
                {
                    Destroy(lockOnCanvasInstance.gameObject);
                }

                // Get the current target's transform
                Transform newTarget = detectedEnemies[currentTargetIndex];

                // Enable target lock
                EnableTarget(newTarget);
            }
        }

        void RepositionCanvas()
        {
            // Calculate the position of the canvas on top of the enemy's head
            Vector3 enemyPosition = currentTarget.position;
            CapsuleCollider enemyCollider = currentTarget.GetComponent<CapsuleCollider>();
            float yOffset = enemyCollider.height / 2f + 1f; 
            Vector3 canvasPosition = new Vector3(enemyPosition.x, enemyPosition.y + yOffset, enemyPosition.z);

            // Instantiate the lock-on canvas if it doesn't exist
            if (lockOnCanvasInstance == null)
            {
                lockOnCanvasInstance = Instantiate(lockOnCanvasPrefab, currentTarget).GetComponent<Canvas>();
                lockOnCanvasInstance.transform.position = canvasPosition;
                lockOnCanvasInstance.enabled = true;
            }
        }
    }
}