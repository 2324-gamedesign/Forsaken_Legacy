using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForsakenLegacy
{
    public class SpawnAreaDamage : MonoBehaviour
    {
        public GameObject damageAreaPrefab;

        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.CompareTag("Bullet"))
            {
                Vector3 areaPos = transform.position;
                areaPos.y = transform.position.y + 0.3f; // Adjust the y position of the damage area to be above the spawn area
                Destroy(other.gameObject);
                Instantiate(damageAreaPrefab, areaPos, transform.rotation);
                Destroy(gameObject, 0.5f); // Destroy the damage area after 0.5 seconds (the duration of the damage area prefab)
            }
            else
            {
                return;
            }
        }
    }
}

