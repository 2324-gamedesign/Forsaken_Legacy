using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForsakenLegacy
{
    public class SpawnAreaDamage : MonoBehaviour
    {
        public GameObject DamageAreaPrefab;
        public AudioSource LandedAudio;
        private bool _hit = false;

        private void Start() {
            Destroy(gameObject, 10f);
        }

        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.CompareTag("Bullet") && !_hit)
            {
                _hit = true;
                LandedAudio.Play();

                Vector3 areaPos = transform.position;
                areaPos.y = transform.position.y + 0.3f; // Adjust the y position of the damage area to be above the spawn area

                Instantiate(DamageAreaPrefab, areaPos, transform.rotation);

                Destroy(other.gameObject);
                Destroy(transform.GetComponentInChildren<Image>().gameObject, 0.5f); // Destroy the damage area image after 0.5 seconds (the duration of the damage area prefab)
            }
        }
    }
}

