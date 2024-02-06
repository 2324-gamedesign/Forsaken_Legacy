using MoreMountains.Feedbacks;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public GameObject player;
    private Vector3 respawnPosition;
    private static RespawnManager instance;
    public static RespawnManager Instance
    {
        get { return instance; }
    }

    public MMFeedbacks respawnFeedback;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        respawnPosition = player.transform.position; // Set initial respawn position to player's current position
    }

    public void SetRespawnPosition(Vector3 position)
    {
        respawnPosition = position;
    }

    public Vector3 GetRespawnPosition()
    {
        return respawnPosition;
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = respawnPosition;
        respawnFeedback.PlayFeedbacks();
    }
}
