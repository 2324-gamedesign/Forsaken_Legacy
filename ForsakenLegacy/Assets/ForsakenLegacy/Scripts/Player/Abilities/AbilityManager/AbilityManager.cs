using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForsakenLegacy
{
    public class AbilityManager : MonoBehaviour
    {
        public static AbilityManager Instance { get; private set; }

        private void Awake() 
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public Ability[] abilities;

        // Example method to unlock the Stun ability
        public void UnlockStunAbility()
        {
            Ability stunAbility = GetAbilityByType(AbilityType.Stun);
            if (stunAbility != null)
            {
                stunAbility.UnlockAbility();
            }
        }

        public Ability GetAbilityByType(AbilityType type)
        {
            foreach (Ability ability in abilities)
            {
                if (ability.type == type)
                {
                    return ability;
                }
            }
            return null;
        }
    }
}

