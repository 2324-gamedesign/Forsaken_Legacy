using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

namespace ForsakenLegacy
{
    public class HealthSystem : MonoBehaviour
    {
        public int maxHealth = 100;
        [SerializeField] private int currentHealth;
        private bool isInvulnerable = false;
        private float timeSinceLastHit = 0.0f;
        public float invulnerabiltyTime = 1.0f; // The time in seconds the player is invulnerable after taking damage.
        
        // References to UI elements
        public Image healthBarFill; // Assign this in the inspector
        public TMP_Text healthText; // Assign this in the inspector

        //Feedbacks
        public MMFeedbacks hitFeedback; // Assign this in the inspector
        public MMFeedbacks deathFeedback; // Assign this in the inspector

        void Start()
        {
            currentHealth = maxHealth; // Set current health to max at the start.
            UpdateHealthUI();
        }
        private void Update()
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

        public void TakeDamage(int damage)
        {
            // Return Conditions
            if (currentHealth <= 0)
            {
                return;
            }
            if (isInvulnerable)
            {
                return;
            }

            //Deal Damage
            isInvulnerable = true;
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
            else
            {
                hitFeedback.PlayFeedbacks(); 
            }
            
            UpdateHealthUI();
        }

        public void Heal(int healingAmount)
        {
            currentHealth += healingAmount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            
            UpdateHealthUI();
        }

        private void Die()
        {
            Debug.Log("Player Died");
            deathFeedback.PlayFeedbacks();
            // Implement what happens when the player dies.
        }

        private void UpdateHealthUI()
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
            healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
        }
        public int GetCurrentHealth()
        {
            return currentHealth;
        }
    }
}
