using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    // private InputAction interactAction;
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
        if (inInteractionArea && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }
}
