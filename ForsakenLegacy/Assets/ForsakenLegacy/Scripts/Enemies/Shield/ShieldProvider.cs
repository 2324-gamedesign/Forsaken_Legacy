using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForsakenLegacy
{
    public class ShieldProvider : MonoBehaviour
    {
        public GameObject[] shieldedObjects; // The object protected by the Shield
        // public float shieldDuration = 5f; // Duration of the barrier in seconds

        public GameObject particlePrefab; // The particle prefab to instantiate when the shield is created

        private List<Shield> shields = new List<Shield>(); // Use a list instead of an array
        private List<GameObject> particles = new List<GameObject>(); // Use a list instead of an array
    
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

                    GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity); // Instantiate particle prefab atshield position

                    // particle.transform.parent = shieldedObject.transform; // Set particle as child of shielded object

                    particle.GetComponent<FollowPathToObject>().Follow(shieldedObject);

                    //add particles to list
                    particles.Add(particle);
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
                foreach (GameObject particle in particles)
                {
                    Destroy(particle); // destroy the particle when the flower is stunned
                }
            }
        }
    }
}
