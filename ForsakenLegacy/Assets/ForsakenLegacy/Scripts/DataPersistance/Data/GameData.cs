using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public GameObject player;
    public int currentHealth;
    public Vector3 playerPosition;

    //Values to start with in new game
    public GameData()
    {
        this.currentHealth = 100;
        playerPosition = new Vector3(0, 1.5f, 0);
    }
}
