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

        // public Dictionary <string, bool> abilities = new Dictionary<string, bool>();

        // public void LoadData(GameData data)
        // {
        //     foreach (string type in abilities.Keys)
        //     {
        //         if (data.abilities.ContainsKey(type))
        //         {
        //             bool unlocked;
        //             data.abilities.TryGetValue(type, out unlocked);
        //             abilities[type] = unlocked;
        //         }
        //     }
        // }
        // public void SaveData(ref GameData data)
        // {
        //     foreach (string type in abilities.Keys)
        //     {
        //         if (data.abilities.ContainsKey(type))
        //         {
        //             data.abilities.Remove(type);
        //         }
        //         data.abilities.Add(type, abilities[type]);
        //     }
        // }

        // // Example method to unlock the Stun ability
        // public void UnlockStunAbility()
        // {
        //     Ability stunAbility = GetAbilityByType(AbilityType.Stun);
        //     if (stunAbility != null)
        //     {
        //         stunAbility.UnlockAbility();
        //     }
        // }

        // public void UnlockAbility(string type)
        // {
        //     if (abilities.ContainsKey(type))
        //     {
        //         abilities[type] = true;
        //     }
        // }

        // // Example method to check if an ability is unlocked
        // public bool IsAbilityUnlocked(string type)
        // {
        //     return abilities.ContainsKey(type) && abilities[type];
        // }
    }
}

