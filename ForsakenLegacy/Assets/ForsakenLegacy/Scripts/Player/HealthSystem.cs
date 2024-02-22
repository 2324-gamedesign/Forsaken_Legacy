using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

namespace ForsakenLegacy
{
    public class HealthSystem : MonoBehaviour, IDataPersistence
    {
        public int maxHealth = 100;
        public bool isDead = false; // Flag to track if the player is dead.
        [SerializeField] private int currentHealth;
        private bool isInvulnerable = false;
        private float timeSinceLastHit = 0.0f;
        public float invulnerabiltyTime = 1.0f; // The time in seconds the player is invulnerable after taking damage.
        
        // References to UI elements
        public Image healthBarFill; // Assign this in the inspector

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


        //Manage Save and Load of HealthPoints thoigh Data peristance interface
        public void LoadData(GameData data)
        {
            this.currentHealth = data.currentHealth;
            UpdateHealthUI();
        }
        public void SaveData(ref GameData data)
        {
            data.currentHealth = this.currentHealth;
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
                isDead = true; // Set the flag to indicate the player is dead.
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
            if(currentHealth > 0)
            {
                isDead = false;
            }
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
        public int GetCurrentHealth()
        {
            return currentHealth;
        }
    }
}
