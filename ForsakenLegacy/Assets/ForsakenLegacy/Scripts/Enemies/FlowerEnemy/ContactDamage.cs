using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForsakenLegacy
{
    public class ContactDamage : MonoBehaviour
    {
        public int damage;
        public int damageDelay = 2;
        public int areaLifetime = 8;
        private GameObject player;

        private void Start() {
            StartCoroutine(DestroyAfterLife());
            player = GameObject.Find("Edea");
        }
        private void OnTriggerEnter(Collider other) {
            if (other.gameObject == player)
            {
                InvokeRepeating("DealDamage", 0.1f, damageDelay);
            }
            else
            {
                return;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("Player"))
            {
                CancelInvoke();
            }
        }

        private void DealDamage()
        {
            player.GetComponent<HealthSystem>().TakeDamage(damage);
        }

        private IEnumerator DestroyAfterLife()
        {
            yield return new WaitForSeconds(areaLifetime);
            Destroy(gameObject);
        }
    }

}
