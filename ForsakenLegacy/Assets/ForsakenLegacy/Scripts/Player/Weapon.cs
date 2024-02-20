using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace ForsakenLegacy{
    public class Weapon : MonoBehaviour
    {
        public MMFeedbacks attack;
        private void OnTriggerEnter(Collider other) 
        {
            if(other.gameObject.CompareTag("Enemy")){
                if(other.gameObject.GetComponent<Damageable>())
                {
                    attack.PlayFeedbacks();
                    other.gameObject.GetComponent<Damageable>().TakeDamage(10, gameObject.transform);
                }
                else return;
            }
            else return;
        }
    }
}
