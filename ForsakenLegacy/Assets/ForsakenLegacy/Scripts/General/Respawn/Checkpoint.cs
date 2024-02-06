using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Save Game Method
            RespawnManager.Instance.SetRespawnPosition(transform.position);
        }
    }
}
