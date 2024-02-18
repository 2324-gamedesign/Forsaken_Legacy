using System.Collections;
using System.Collections.Generic;
using ForsakenLegacy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Arena : MonoBehaviour
{
    // public GameObject player;
    public GameObject[] doors;
    public Dictionary<string, bool> enemies;

    private void Start() {
        enemies = new Dictionary<string, bool>();
        
        //Populate the enemies dictionary
        Enemy[] enemyArray = GetComponentsInChildren<Enemy>();
        foreach (Enemy enemy in enemyArray)
        {
            enemies.Add(enemy.GetID(), enemy.isDead);
            Debug.Log("Added enemy " + enemy.GetID() + " to the dictionary");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player"))
        {
            CloseDoors();
        }
    }
    
    // Update the status of the enemy in the dictionary
    public void UpdateEnemyStatus(string enemyID, bool isDead)
    {
        enemies[enemyID] = isDead;
        CheckAllEnemiesDead();
    }

    private void CheckAllEnemiesDead()
    {
        foreach (bool isDead in enemies.Values)
        {
            if (!isDead)
            {
                return;
            }
        }
        OpenDoors();
    }

    public void CloseDoors()
    {
        Guardian[] guardians = GetComponentsInChildren<Guardian>();
        //Close the doors
        foreach(GameObject door in doors)
        {
            door.GetComponent<Door>().CloseDoor();
        }

        //Enemies start attacking
        foreach(Guardian guardian in guardians)
        {
            guardian.StartPursuit();
        }
    }
    private void OpenDoors()
    {
        //Open the doors
        foreach(GameObject door in doors)
        {
            door.GetComponent<Door>().OpenDoor();
        }
    }
}
