using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace ForsakenLegacy
{
    public class Stunnable : MonoBehaviour
    {
        public bool isStunned = false;
        public MMFeedbacks feedbackStunStart;
        public MMFeedbacks feedbackStunEnd;

        public void Stun()
        {
            // Implement your stun logic here, for example, disabling enemy movement
            StartCoroutine(StunCoroutine(1));
        }

        public void Stun(float duration)
        {
            // Implement your stun logic here, for example, disabling enemy movement
            StartCoroutine(StunCoroutine(duration));
        }

        IEnumerator StunCoroutine(float duration)
        {
            isStunned = true;
            feedbackStunStart.PlayFeedbacks();

            yield return new WaitForSeconds(duration);

            StunEnd();
        }
        public void StunEnd()
        {
            feedbackStunEnd.PlayFeedbacks();
            isStunned = false;
        }
    }
}

