using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object has the specified ground tag
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Environment"))
        {
            // Destroy the GameObject when it collides with the ground
            Destroy(gameObject);
        }
    }

    private void Start() {
        StartCoroutine(DestroyBullet());
    }

    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
