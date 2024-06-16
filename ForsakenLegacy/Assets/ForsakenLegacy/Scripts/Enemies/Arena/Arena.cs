using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using ForsakenLegacy;
using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class Arena : MonoBehaviour, IDataPersistence
{
    public Door[] doors;
    public Dictionary<string, bool> enemies;
    private Collider playerCollider;
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


    private void Start() {
        //initialize enemy dictionary
        enemies = new Dictionary<string, bool>();
        playerCollider = GameObject.Find("Edea").GetComponent<Collider>();
        
        //Populate the enemies dictionary
        Enemy[] enemyArray = GetComponentsInChildren<Enemy>();
        foreach (Enemy enemy in enemyArray)
        {
            enemies.Add(enemy.GetID(), enemy.isDead);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other == playerCollider && !isInProgress && !cleared)
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
        MusicManager.Instance.Combat();

        //Close the doors
        foreach(Door door in doors)
        {
            door.CloseDoor();
        }

        //Enemies start attacking
        foreach (Guardian guardian in GetComponentsInChildren<Guardian>())
        {
            guardian.Spawn();
        }
    }
    private void OpenDoors()
    {
        cleared = true;
        isInProgress = false;
        MusicManager.Instance.Exploration();
       
        //Open the doors
        foreach(Door door in doors)
        {
            door.OpenDoor();
        }
    }
}
