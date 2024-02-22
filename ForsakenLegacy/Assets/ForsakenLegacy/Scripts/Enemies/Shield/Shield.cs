using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ForsakenLegacy
{
    public class Shield : MonoBehaviour
    {
        public bool isEnemy = true;

        private void Start() {
            if(isEnemy)
            {
                GetComponentInParent<Damageable>().enabled = false;
            }
        }
        public void DisableShield()
        {
            if(isEnemy)
            {
                GetComponentInParent<Damageable>().enabled = true;
            }

            gameObject.SetActive(false);
        }

        // public void EnableShield()
        // {
        //     if(isEnemy)
        //     {
        //         GetComponentInParent<Damageable>().enabled = true;
        //     }

        //     gameObject.SetActive(true);
        // }
    }
}

