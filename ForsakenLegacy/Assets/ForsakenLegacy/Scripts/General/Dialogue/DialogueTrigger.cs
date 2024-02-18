using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    public bool canProceed = true;
    public int nextDialogue = 0;
    public Dialogue[] dialogues;
    private bool inInteractionArea = false;

    //UI Elements
    public TMP_Text tutorial;
   
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player"))
        {

            inInteractionArea = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player"))
        {
            tutorial.text = "";
            inInteractionArea = false;
        }
    }

    private void Update()
    {
        if (inInteractionArea && !DialogueManager.Instance.isInDialogue && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TriggerDialogue();
        }
        
        if(inInteractionArea && !DialogueManager.Instance.isInDialogue)
        {
            tutorial.text = "Press E to talk";
        }
    }

    public void TriggerDialogue()
    {
        tutorial.text = "";
        if(canProceed && nextDialogue < dialogues.Length)
        {
           nextDialogue += 1;
        }
        
        DialogueManager.Instance.StartDialogue(nextDialogue, dialogues);
    }
}
