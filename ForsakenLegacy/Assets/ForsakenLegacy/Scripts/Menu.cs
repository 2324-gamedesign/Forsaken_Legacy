using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    private InputAction menuAction;
    public PlayerInput _playerInput;
    public Canvas menu;
    private bool menuIsOn = false;

    // Start is called before the first frame update
    void Start()
    {
        menuAction = _playerInput.actions.FindAction("Menu");

        menuAction.performed += OnMenuCalled;  
    }

    // Update is called once per frame
    void OnMenuCalled(InputAction.CallbackContext context){
        if(menuIsOn){
            menu.enabled = false;
            menuIsOn = false;
        }
        else{
            menu.enabled = true;
            menuIsOn = true;
        }
    }
}
