using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace ForsakenLegacy
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField] private int currentHP;
        public int maxHP = 10;
        private Collider _collider;
        private bool isInvulnerable;
        private float timeSinceLastHit;
        private readonly float invulnerabiltyTime = 0.1f;

        // private float hitForwardRotation = 360.0f;
        // private float hitAngle = 360.0f;

        public MMFeedbacks hitFeedback;
        public MMFeedbacks deathFeedback;

        private void Start() {
            ResetDamage();
            _collider = GetComponent<Collider>();
        }

        void Update()
        {
            if (isInvulnerable)
            {
                timeSinceLastHit += Time.deltaTime;
                if (timeSinceLastHit > invulnerabiltyTime)
                {
                    timeSinceLastHit = 0.0f;
                    isInvulnerable = false;
                }
            }
        }

        public void ResetDamage()
        {
            currentHP = maxHP;
            isInvulnerable = false;
            timeSinceLastHit = 0.0f;
        }
        public void SetColliderState(bool enabled)
        {
            _collider.enabled = enabled;
        }

        public void TakeDamage(int damage, Transform damageSource)
        {
            // Vector3 forward = transform.forward;
            // forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;
            // //we project the direction to damager to the plane formed by the direction of damage
            // Vector3 positionToDamager = damageSource.position - transform.position;
            // positionToDamager -= transform.up * Vector3.Dot(transform.up, positionToDamager);

            // Return Conditions
            if (currentHP <= 0)
            {
                return;
            }
            if (isInvulnerable)
            {
                return;
            }
            // if (Vector3.Angle(forward, positionToDamager) > hitAngle * 0.5f)
            // {
            //     return;
            // }

            //Deal Damage
            isInvulnerable = true;
            currentHP -= damage;

            if (currentHP <= 0)
            {
                currentHP = 0;
                OnDeath();
            }
            else
            {
                hitFeedback.PlayFeedbacks();
            }
        }

        private void OnDeath()
        {
            gameObject.GetComponent<Enemy>().OnDeath();
            hitFeedback.PlayFeedbacks(); 
            deathFeedback.PlayFeedbacks();
        }
    }

}
