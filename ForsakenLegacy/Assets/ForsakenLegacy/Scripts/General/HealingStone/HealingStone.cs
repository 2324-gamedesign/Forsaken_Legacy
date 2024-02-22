using System.Collections;
using System.Collections.Generic;
using ForsakenLegacy;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HealingStone : MonoBehaviour
{
    public GameObject player;
    public TMP_Text tutorial;
    public GameObject blood;
    private bool inInteractionArea;
    public bool isActive = true;
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player") && isActive)
        {
            tutorial.text = "Press E to Collect";
            inInteractionArea = true;
        }
    }

    private void Update()
    {
        if (inInteractionArea && isActive && Keyboard.current.eKey.wasPressedThisFrame)
        {
            player.GetComponent<HealthSystem>().IncreasePotions(2);
            isActive = false;
            blood.SetActive(false);
            tutorial.text = "";
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            inInteractionArea = false;
            tutorial.text = "";
        }
    }
}
