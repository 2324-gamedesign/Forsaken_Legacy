using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForsakenLegacy
{
    public class ShieldProvider : MonoBehaviour
    {
        public GameObject shieldedObject; // The object protected by the Shield
        public float shieldDuration = 5f; // Duration of the barrier in seconds
    
        public Shield shield;
    
        void Start()
        {
            // Get the Shield component from the shielded object
            if(shieldedObject != null)
            {
                shield = shieldedObject.GetComponentInChildren<Shield>();
            }
        }
    
        // Function to stun the flower
        public void Stun()
        {
            if (!GetComponent<Stunnable>().isStunned && shield != null)
            {
                shield.DisableShield(); // Disable the Shield when the flower is stunned
            }
        }
    }
}
