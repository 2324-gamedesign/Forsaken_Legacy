using Ilumisoft.VisualStateMachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public StateMachine stateMachine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetNeutralState()
    {
        stateMachine.Trigger("Neutral");
    }

    public void SetDialogueState()
    {
        stateMachine.Trigger("Dialogue");
    }
    public void QuitGame()
    {

    }
}

