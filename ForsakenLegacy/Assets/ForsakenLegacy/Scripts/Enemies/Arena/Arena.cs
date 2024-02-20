using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using ForsakenLegacy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Arena : MonoBehaviour, IDataPersistence
{
    public GameObject[] doors;
    public Dictionary<string, bool> enemies;
    public bool cleared = false;
    public bool isInProgress = false;

    public string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = (System.Guid.NewGuid().ToString());
    }


    public void LoadData(GameData data)
    {
        data.clearedArenas.TryGetValue(id, out cleared);
        if (cleared)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            foreach (Enemy enemy in GetComponentsInChildren<Enemy>())
            {
                enemy.gameObject.SetActive(true);
                enemy.OnRespawn();
            }
        }
    }
    public void SaveData(ref GameData data)
    {
        if (data.clearedArenas.ContainsKey(id))
        {
            data.clearedArenas.Remove(id);
        }
        data.clearedArenas.Add(id, cleared);
    }


    private void Awake() {
        //initialize enemy dictionary
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
        isInProgress = true;

        //Close the doors
        foreach(GameObject door in doors)
        {
            door.GetComponent<Door>().CloseDoor();
        }

        //Enemies start attacking
        foreach (GuardianArena guardian in GetComponentsInChildren<GuardianArena>())
        {
            guardian.StartPursuit();
        }
    }
    private void OpenDoors()
    {
        cleared = true;
        isInProgress = false;
        gameObject.SetActive(false);
       
        //Open the doors
        foreach(GameObject door in doors)
        {
            door.GetComponent<Door>().OpenDoor();
        }
    }
}
