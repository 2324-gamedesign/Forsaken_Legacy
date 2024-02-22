using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForsakenLegacy
{
    public class ShieldProvider : MonoBehaviour
    {
        public GameObject[] shieldedObjects; // The object protected by the Shield
        // public float shieldDuration = 5f; // Duration of the barrier in seconds
        [SerializeField] private List<Shield> shields = new List<Shield>(); // Use a list instead of an array
    
        void Start()
        {
            // Get the Shield component from the shielded object
            if(shieldedObjects != null)
            {
                foreach (GameObject shieldedObject in shieldedObjects)
                {
                    Shield shield = shieldedObject.GetComponentInChildren<Shield>();
                    if (shield != null)
                    {
                        shields.Add(shield); // Add the shield component to the list
                    }
                } 
            }
        }
    
        // Function to stun the flower
        public void Stun()
        {
            if (!GetComponent<Stunnable>().isStunned && shields != null)
            {
                foreach (Shield shield in shields)
                {
                    shield.DisableShield(); // disable the Shield when the flower is stunned
                }
            }
        }
    }
}
