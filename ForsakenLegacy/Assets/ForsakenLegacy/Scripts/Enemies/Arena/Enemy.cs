using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private string id;
    public bool isDead = false;
    public MMFeedbacks respawnFeedback;

    private Arena arena;
    

    private void Start()
    {
        GenerateGuid();
        arena = gameObject.GetComponentInParent<Arena>();
    }
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    public void OnDeath()
    {
        isDead = true;
        arena.UpdateEnemyStatus(id, isDead); // Notify the Arena script about the enemy's death
    }

    public void OnRespawn()
    {
        respawnFeedback.PlayFeedbacks();
    }

    public string GetID()
    {
        return id;
    }
}
