using System.Collections;
using System.Collections.Generic;
using ForsakenLegacy;
using UnityEngine;

public class GuardianWeapon : MonoBehaviour
{
    private Collider weaponCollider;

    public int damage = 20;
    // Start is called before the first frame update
    void Start()
    {
        weaponCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) 
        {
            if(other.GetComponent<HealthSystem>())
            {
                other.GetComponent<HealthSystem>().TakeDamage(damage);
            }
            else return;
        }
        else return;
    }
}
