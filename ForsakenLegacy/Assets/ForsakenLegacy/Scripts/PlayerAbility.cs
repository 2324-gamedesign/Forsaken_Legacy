using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ForsakenLegacy
{
    public class PlayerAbility : MonoBehaviour
    {
        public PlayerInput _playerInput;
        private CharacterController characterController;
        private bool isAttacking;
        public bool isInAbility;
        public Canvas dashCanvas;
        public Image dashIndicator;
        public GameObject dashPoint;
        private bool isIndicatorActive = false;

        private InputAction dashAction;
        private InputAction confirmAction;
        private InputAction cancelAction;

        private Vector3 mousePosition;
        private Ray ray;

        void Start()
        {
            isAttacking = gameObject.GetComponent<PlayerController>().isAttacking;
            characterController = GetComponent<CharacterController>();
            dashCanvas.enabled = false;

            dashAction = _playerInput.actions.FindAction("Dash");
            confirmAction = _playerInput.actions.FindAction("Confirm");
            cancelAction = _playerInput.actions.FindAction("Cancel");

            dashAction.performed += OnDashTargetingStart;
            confirmAction.performed += OnDashConfirmed;
            cancelAction.performed += OnDashTargetingEnd;
        }

        void Update()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (isIndicatorActive) UpdateIndicatorPosition();
        }

        void OnDashTargetingStart(InputAction.CallbackContext context)
        {
            if (!isAttacking)
            {
                isInAbility = true;
                SetDashCanvasState(true);
            }
        }

        void OnDashTargetingEnd(InputAction.CallbackContext context)
        {
            if (isInAbility)
            {
                SetDashCanvasState(false);
            }
        }

        void OnDashConfirmed(InputAction.CallbackContext context)
        {
            DashPlayer();
        }

        void UpdateIndicatorPosition()
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                mousePosition = new Vector3(hit.point.x, 0, hit.point.z);
            }

            Quaternion rotation = Quaternion.LookRotation(mousePosition - transform.position);
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            dashCanvas.transform.rotation = Quaternion.Lerp(dashCanvas.transform.rotation, rotation, Time.deltaTime * 5f);
        }

        void SetDashCanvasState(bool state)
        {
            if (dashCanvas && dashPoint)
            {
                dashCanvas.enabled = state;
                isIndicatorActive = state;
            }
        }

        public void DashPlayer()
        {
            if (dashCanvas.enabled && isInAbility)
            {
                gameObject.transform.position = dashPoint.transform.position;
                SetDashCanvasState(false);
                isInAbility = false;
            }
        }
    }
}


