using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;

public class DashAbility : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _controller;

    private InputAction dashAction;
    public float dashDistance = 5.0f;
    private float dashDuration = 0.1f;
    public bool isDashing = false;
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
        Animator.StringToHash("Speed");
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

        _animator.Play("Dash");
        FadeOut();

        trail.SetActive(true);
        dashParticles.PlayFeedbacks();

        // Store the current position, rotation, and y-coordinate
        Vector3 startPosition = transform.position;
        float startY = transform.position.y;

        dashDirection = transform.forward;
        _controller.enabled = false;

        // Perform a sphere cast in the dash direction
        if (Physics.SphereCast(startPosition, _controller.radius, dashDirection, out RaycastHit hit, dashDistance, LayerMask.GetMask("Ground", "Environment")))
        {
            // Adjust the player's position to the point of contact with the obstacle
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

        FadeIn();
        dashCooldownImage.fillAmount = 1;

        isDashing = false;
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
        
        Debug.Log("fade out");
    }

    public void FadeIn() {
        Renderer[] renderer = GetComponentsInChildren<Renderer>();

        foreach(Renderer i in renderer)
        i.enabled = true;

        Debug.Log("fade in");
    }
}
