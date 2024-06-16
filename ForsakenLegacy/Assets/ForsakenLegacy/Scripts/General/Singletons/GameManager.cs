using Ilumisoft.VisualStateMachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public StateMachine globalStateMachine;
    public StateMachine playerStateMachine;

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
        globalStateMachine.Trigger("Neutral");
    }

    public void SetDialogueState()
    {
        globalStateMachine.Trigger("Dialogue");
    }

    public void SetCutSceneState()
    {
        globalStateMachine.Trigger("CutScene");
    }

    public void SetMenuState()
    {
        globalStateMachine.Trigger("Menu");
    }

    public void PauseGame()
    {
        //Set Game Time to 0
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        //Set Game Time to 1
        Time.timeScale = 1;
    }

    //PlayerStateMachine
    public void SetMoveState()
    {
        if(playerStateMachine.CurrentState == "Attack")
        {
            playerStateMachine.Trigger("Move");
        }
        else playerStateMachine.Trigger("MoveAbility");
    }

    public void SetAttackState()
    {
        playerStateMachine.Trigger("Attack");
    }

    public void SetAbilityState()
    {
        playerStateMachine.Trigger("Ability");
    }
    
    public void SetDeathState()
    {
        playerStateMachine.Trigger("Death");
    }
}

