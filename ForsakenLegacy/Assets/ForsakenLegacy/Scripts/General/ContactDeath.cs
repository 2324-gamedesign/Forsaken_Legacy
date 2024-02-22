using System.Collections;
using System.Collections.Generic;
using ForsakenLegacy;
using UnityEngine;

public class ContactDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the collider is the player
        {
            other.GetComponent<HealthSystem>().TakeDamage(100); // Call the Die method when the player collides with the object
        }
    }
}
