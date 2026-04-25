using System;
using Unity.Burst.Intrinsics;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
        public bool attack;
        public bool reload;
		public bool roll;
        public float switchWeapon;

        [Header("Movement Settings")]
		public bool analogMovement;

        public event Action OnAttackStarted;
        public event Action OnAttackEnded;

        public event Action OnReloadStarted;
        public event Action OnReloadEnded;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
				LookInput(value.Get<Vector2>());
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

        public void OnAttack(InputValue value)
        {
            bool isPressed = value.isPressed;

            if (isPressed && !attack)
                OnAttackStarted?.Invoke();
            else if (!isPressed && attack)
                OnAttackEnded?.Invoke();

            attack = isPressed;
        }

        public void OnReload(InputValue value)
        {
            bool isPressed = value.isPressed;

            if (isPressed && !reload)
                OnReloadStarted?.Invoke();
            else if (!isPressed && reload)
                OnReloadEnded?.Invoke();

            reload = isPressed;
        }

        public void OnSwitchWeapon(InputValue value)
        {
            SwitchWeaponInput(value.Get<float>());
        }

        public void OnRoll(InputValue value)
        {
            RollInput(value.isPressed);
        }


#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

        public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

        public void SwitchWeaponInput(float newWeaponSwitchState)
        {
            switchWeapon = newWeaponSwitchState;
        }

        public void RollInput(bool newRollState)
        {
            roll = newRollState;
        }
    }
	
}