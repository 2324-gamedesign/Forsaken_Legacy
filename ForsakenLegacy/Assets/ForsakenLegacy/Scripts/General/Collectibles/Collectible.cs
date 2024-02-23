using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using ForsakenLegacy;

public class Collectible : MonoBehaviour
{
    public GameObject player;
    private bool inInteractionArea;
    public bool isActive = true;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player") && isActive)
        {
            DialogueManager.Instance.EditTutorialText("Press E to Collect");
            inInteractionArea = true;
        }
    }

    private void Update()
    {
        if (inInteractionArea && isActive && Keyboard.current.eKey.wasPressedThisFrame)
        {
            player.GetComponent<StunAbility>().Ability.unlocked = true;

            isActive = false;
            gameObject.SetActive(false);
            DialogueManager.Instance.EditTutorialText("");
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            inInteractionArea = false;
            DialogueManager.Instance.EditTutorialText("");
        }
    }
}

