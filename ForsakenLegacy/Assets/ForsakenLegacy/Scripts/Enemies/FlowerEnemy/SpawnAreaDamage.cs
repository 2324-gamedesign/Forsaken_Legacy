using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForsakenLegacy
{
    public class SpawnAreaDamage : MonoBehaviour
    {
        public GameObject DamageAreaPrefab;
        public AudioSource LandedAudio;

        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.CompareTag("Bullet"))
            {
                LandedAudio.Play();
                Debug.Log("Played Sound");

                Vector3 areaPos = transform.position;
                areaPos.y = transform.position.y + 0.3f; // Adjust the y position of the damage area to be above the spawn area

                Instantiate(DamageAreaPrefab, areaPos, transform.rotation);

                Destroy(other.gameObject);
                Destroy(gameObject, 0.5f); // Destroy the damage area after 0.5 seconds (the duration of the damage area prefab)
            }
            else
            {
                Destroy(gameObject, 5f); // Destroy the damage area after 5 seconds if the bullet never reached it (enemy probably died before shooting)
                return;
            }
        }
    }
}

