using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private string id;
    public bool isDead = false;

    private Arena arena;

    [ContextMenu("Generate guid for ID")]
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

    public string GetID()
    {
        return id;
    }
}
