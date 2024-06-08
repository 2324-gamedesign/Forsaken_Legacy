using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using System.Collections;

namespace ForsakenLegacy
{
    public class HealthSystem : MonoBehaviour, IDataPersistence
    {
        public int MaxHealth = 100;
        public bool IsDead = false; // Flag to track if the player is dead.
        [SerializeField] private int _currentHealth;
        private bool _isInvulnerable = false;
        private float _timeSinceLastHit = 0.0f;
        private readonly float _invulnerabiltyTime = 1.0f; // The time in seconds the player is invulnerable after taking damage.

        [Header("Potions")]
        private int _healingPotions;
        public int MaxPotions = 2;
        public int HealingAmount = 10;
        private InputAction _healAction;
        private PlayerInput _playerInput;
        
        [Header("Health Bar UI")]
        public Image healthBarFill;
        public Image healthBarFillDelay;
        public TMP_Text potionNumberText; 
        public Image potionFill;

        [Header("Health Bar Feedbacks")]
        public MMFeedbacks hitFeedback; 
        public MMFeedbacks deathFeedback; 

        void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
            _healAction = _playerInput.actions["Heal"];
            _healAction.performed += Heal;

            _currentHealth = MaxHealth; // Set current health to max at the start.
            UpdateHealthUI();
        }
    
        private void Update()
        {
            if (_isInvulnerable)
            {
                _timeSinceLastHit += Time.deltaTime;
                if (_timeSinceLastHit > _invulnerabiltyTime)
                {
                    _timeSinceLastHit = 0.0f;
                    _isInvulnerable = false;
                }
            }
        }


        //Manage Save and Load of HealthPoints thoigh Data peristance interface
        public void LoadData(GameData data)
        {
            this._currentHealth = data.currentHealth;
            this._healingPotions = data.healingPotions;
            UpdatePotionUI();
            UpdateHealthUI();
        }
        public void SaveData(ref GameData data)
        {
            data.currentHealth = this._currentHealth;
            data.healingPotions = this._healingPotions;
        }


        public void TakeDamage(int damage)
        {
            // Return Conditions
            if (_currentHealth <= 0)
            {
                return;
            }
            if (_isInvulnerable)
            {
                return;
            }

            //Deal Damage
            _isInvulnerable = true;
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                IsDead = true; // Set the flag to indicate the player is dead.
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
            if(_healingPotions <= 0 || _currentHealth == MaxHealth)
            {
                return;
            }

            _healingPotions -= 1; // Decrement the number of healing potions by 1.
            _currentHealth += HealingAmount;
            if (_currentHealth > MaxHealth)
            {
                _currentHealth = MaxHealth;
            }
            
            UpdateHealthUI();
            UpdatePotionUI();
        }

        public bool IncreasePotions(int potions)
        {
            if (_healingPotions + potions > MaxPotions)
            {
                return false; // Return false if the number of potions to increase exceeds the maximum.
            }

            _healingPotions += potions;
            UpdatePotionUI(); // Update the UI to reflect the new number of healing potions.
            return true; // Return true if the potions were successfully increased.
        }

        private void Die()
        {
            Debug.Log("--- You Died ---");
            deathFeedback.PlayFeedbacks();
        }
        public void UpdatePotionUI()
        {
            // Set text of current potions to teh number of current potion "/" the number of max potions
            potionNumberText.text = _healingPotions.ToString() + "/" + MaxPotions.ToString();

            StartCoroutine(PotionFillUI());
        }

        private void UpdateHealthUI()
        {
            if(_currentHealth > 0)
            {
                IsDead = false;
            }
            StartCoroutine(HealthBarFillUI());
        }
        private IEnumerator HealthBarFillUI()
        {
            //gradually update the healthbar through time
            float fillAmount = (float)_currentHealth / MaxHealth;
            if(fillAmount < healthBarFill.fillAmount)
            {
                while (healthBarFill.fillAmount > fillAmount)
                {
                    healthBarFill.fillAmount -= 0.01f;
                    yield return null;
                }
                yield return new WaitForSeconds(1f);
                while (healthBarFillDelay.fillAmount > healthBarFill.fillAmount)
                {
                    healthBarFillDelay.fillAmount -= 0.01f;
                    yield return null;
                }
            }
            else
            {
                while (healthBarFill.fillAmount < fillAmount)
                {
                    healthBarFill.fillAmount += 0.005f;
                    yield return null;
                }
                healthBarFillDelay.fillAmount = healthBarFill.fillAmount;
            }
        }

        private IEnumerator PotionFillUI()
        {
            //gradually update the healthbar through time
            float fillAmount = (float)_healingPotions / MaxPotions;
            if(fillAmount < potionFill.fillAmount)
            {
                while (potionFill.fillAmount > fillAmount)
                {
                    potionFill.fillAmount -= 0.01f;
                    yield return null;
                }
            }
            else
            {
                while (potionFill.fillAmount < fillAmount)
                {
                    potionFill.fillAmount += 0.01f;
                    yield return null;
                }
            }
        }

        public int GetCurrentHealth()
        {
            return _currentHealth;
        }
        public int GetCurrentPotions()
        {
            return _healingPotions;
        }
    }
}
