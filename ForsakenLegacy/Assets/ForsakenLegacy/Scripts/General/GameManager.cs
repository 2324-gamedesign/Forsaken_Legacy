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

    public void SetCutSceneState()
    {
        stateMachine.Trigger("CutScene");
    }
    public void QuitGame()
    {

    }
    public void StopTime()
    {
        //Set Game Time to 0
        Time.timeScale = 0;
    }
    public void StartTime()
    {
        //Set Game Time to 1
        Time.timeScale = 1;
    }
}

