using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    public bool canProceed = true;
    public int nextDialogue = 0;
    public Dialogue[] dialogues;
    private bool inInteractionArea = false;
   
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player"))
        {
            inInteractionArea = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player"))
        {
            inInteractionArea = false;
        }
    }

    private void Update()
    {
        if (inInteractionArea && !DialogueManager.Instance.isInDialogue && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        if(canProceed && nextDialogue < dialogues.Length)
        {
           nextDialogue += 1;
        }
        
        DialogueManager.Instance.StartDialogue(nextDialogue, dialogues);
    }
}
