using System.Collections;
using System.Collections.Generic;
using ForsakenLegacy;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currentHealth;
    public int healingPotions;
    public Vector3 playerPosition;
    public SerializableDictionary<string, bool> abilities;
    public SerializableDictionary<string, bool> clearedArenas;

    //Values to start with in new game
    public GameData()
    {
        this.currentHealth = 100;
        this.healingPotions = 0;
        playerPosition = new Vector3(70, 18, 33);
        abilities = new SerializableDictionary<string, bool>();
        clearedArenas = new SerializableDictionary<string, bool>();
    }
}
