using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class DashAbility : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _controller;

    private InputAction dashAction;
    public float dashDistance = 5.0f;
    private float dashDuration = 1f;
    public bool isDashing = false;
    private bool canDash = true;
    private Vector3 dashDirection;

    public float dashCooldown = 5f;
    public Image dashCooldownImage; 

    // Start is called before the first frame update
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();
        dashAction = _playerInput.actions.FindAction("Dash");
        dashAction.performed += OnDashPerformed;
    }

    void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (!isDashing && canDash)
        {
            StartCoroutine(PerformDash());
        }
    }
    private IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;

        FadeOut();

        // Store the current position, rotation, and y-coordinate
        Vector3 startPosition = transform.position;
        float startY = transform.position.y;
        // Calculate the dash direction based on input or current rotation if not moving
        dashDirection = transform.forward;
        // Disable character controller while dashing
        _controller.enabled = false;

        // Perform a raycast in the dash direction
        RaycastHit hit;
        if (Physics.Raycast(startPosition, dashDirection, out hit, dashDistance, LayerMask.GetMask("Ground","Environment")))
        {
            // Adjust the player's position to the point of contact with the obstacle
            transform.position = hit.point;
        }
        else
        {
            // If no obstacle, move the player instantly in the dash direction
            transform.position += dashDirection * dashDistance;
        }

        // Reset y-coordinate to the initial value
        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        _controller.enabled = true;

        // Reset the UI image's fill amount to full
        dashCooldownImage.fillAmount = 1;

        // Wait for the dash duration
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        // Start the cooldown countdown
        StartCoroutine(DashCooldown());
    }
    private IEnumerator DashCooldown()
    {
        float elapsedTime = 0f;
        // Loop until the cooldown duration is reached
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
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(GetComponentInChildren<Renderer>().material.DOFade(0, 0.2f).SetEase(Ease.OutQuint));
        mySequence.AppendInterval(1f);
        mySequence.Rewind();
    }
}
