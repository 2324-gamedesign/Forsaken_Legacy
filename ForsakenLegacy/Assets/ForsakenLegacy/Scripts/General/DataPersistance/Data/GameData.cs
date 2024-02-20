using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currentHealth;
    public Vector3 playerPosition;
    public SerializableDictionary<string, bool> clearedArenas;

    //Values to start with in new game
    public GameData()
    {
        this.currentHealth = 100;
        playerPosition = new Vector3(0, 1.5f, 0);
        clearedArenas = new SerializableDictionary<string, bool>();
    }
}
