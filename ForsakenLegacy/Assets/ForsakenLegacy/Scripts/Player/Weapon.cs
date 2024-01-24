using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForsakenLegacy{
    public class Weapon : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other) 
        {
            if(other.gameObject.CompareTag("Enemy")){
                if(other.gameObject.GetComponent<Damageable>())
                {
                    other.gameObject.GetComponent<Damageable>().TakeDamage(10, gameObject.transform);
                    Debug.Log("Hit Enemy");
                }
                else return;
            }
            else return;
        }
    }
}
