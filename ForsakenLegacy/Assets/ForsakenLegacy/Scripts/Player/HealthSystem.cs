using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ForsakenLegacy
{
    public class HealthSystem : MonoBehaviour
    {
        public int maxHealth = 100;
        private int currentHealth;
        
        // References to UI elements
        public Image healthBarFill; // Assign this in the inspector
        public TMP_Text healthText; // Assign this in the inspector

        void Start()
        {
            currentHealth = maxHealth; // Set current health to max at the start.
            UpdateHealthUI();
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
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
            // Implement what happens when the player dies.
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }
        
        private void UpdateHealthUI()
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
            healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
        }
    }
}
