using UnityEngine;
using System;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using System.Collections;

namespace ForsakenLegacy
{
	public class InputController : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public bool sprint;
		public bool attack;
		public int noOfClicks;
    	public float lastClickedTime;
		public float clickCooldown = 0.5f;


		[Header("Movement Settings")]
		public bool analogMovement;

	

		#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
			public void OnMove(InputValue value)
			{
				MoveInput(value.Get<Vector2>());
			}
			public void OnSprint(InputValue value)
			{
				SprintInput(value.isPressed);
			}
			public void OnAttack(InputValue value)
			{
				AttackInput(value.isPressed);
			}
		#endif


		public void MoveInput(Vector2 newMoveDirection)
			{
				move = newMoveDirection;
			} 
		public void SprintInput(bool newSprintState)
			{
				sprint = newSprintState;
			}
		public void AttackInput(bool newAttackState)
    		{
				attack = newAttackState;
				if (newAttackState && Time.time - lastClickedTime > clickCooldown)
            	{
            	    // Register a new click only if enough time has passed
            	    lastClickedTime = Time.time;
            	    noOfClicks++;
            	}
			}

	}
}