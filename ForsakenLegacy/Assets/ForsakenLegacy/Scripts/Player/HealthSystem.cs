using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;
using UnityEngine.InputSystem;

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

        public int healingPotions;
        public int healingAmount = 10;
        private InputAction healAction;
        private PlayerInput _playerInput;
        
        // References to UI elements
        public Image healthBarFill; // Assign this in the inspector
        public TMP_Text potionNumberText; // Assign this in the inspector

        //Feedbacks
        public MMFeedbacks hitFeedback; // Assign this in the inspector
        public MMFeedbacks deathFeedback; // Assign this in the inspector

        void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
            healAction = _playerInput.actions["Heal"];
            healAction.performed += Heal;

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
            this.healingPotions = data.healingPotions;
            UpdatePotionUI();
            UpdateHealthUI();
        }
        public void SaveData(ref GameData data)
        {
            data.currentHealth = this.currentHealth;
            data.healingPotions = this.healingPotions;
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

        public void Heal(InputAction.CallbackContext context) 
        {
            if(healingPotions <= 0 || currentHealth == maxHealth)
            {
                return;
            }

            healingPotions -= 1; // Decrement the number of healing potions by 1.
            currentHealth += healingAmount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            
            UpdateHealthUI();
            UpdatePotionUI();
        }

        public void IncreasePotions(int potions)
        {
            healingPotions += potions;
            UpdatePotionUI(); // Update the UI to reflect the new number of healing potions.
        }

        private void Die()
        {
            Debug.Log("Player Died");
            deathFeedback.PlayFeedbacks();
            // Implement what happens when the player dies.
        }
        public void UpdatePotionUI()
        {
            potionNumberText.text = healingPotions.ToString(); // Update the UI text with the new number of healing potions.
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
