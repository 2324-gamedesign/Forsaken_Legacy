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
                if(GetComponentInParent<Damageable>() != null)
                {
                    GetComponentInParent<Damageable>().enabled = false;
                }

                if(GetComponentInParent<Stunnable>() != null)
                {
                    GetComponentInParent<Stunnable>().isStunnable = false;
                }
            }
        }
        public void DisableShield()
        {
            if(isEnemy)
            {
                if(GetComponentInParent<Damageable>() != null)
                {
                    GetComponentInParent<Damageable>().enabled = true;
                }

                if(GetComponentInParent<Stunnable>() != null)
                {
                    GetComponentInParent<Stunnable>().isStunnable = true;
                }
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

