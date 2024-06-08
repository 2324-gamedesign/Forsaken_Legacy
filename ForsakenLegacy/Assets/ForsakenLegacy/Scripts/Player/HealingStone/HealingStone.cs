using System.Collections;
using System.Collections.Generic;
using ForsakenLegacy;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HealingStone : MonoBehaviour
{
    private GameObject _player;
    public GameObject blood;
    private bool inInteractionArea;
    public bool isActive = true;

    private void Start() {
        _player = GameObject.Find("Edea");
    }

    private void OnTriggerEnter(Collider other) {
        if(other == _player.GetComponent<Collider>() && isActive)
        {
            DialogueManager.Instance.EditTutorialText("Press E to Collect");
            inInteractionArea = true;
        }
    }

    private void Update()
    {
        if (inInteractionArea && isActive && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if(_player.GetComponent<HealthSystem>().IncreasePotions(2))
            {
                isActive = false;
                blood.SetActive(false);
                DialogueManager.Instance.EditTutorialText("");
            }
            else
            {
                DialogueManager.Instance.EditTutorialText("Blood Full");
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other == _player.GetComponent<Collider>())
        {
            inInteractionArea = false;
            DialogueManager.Instance.EditTutorialText("");
        }
    }
}
