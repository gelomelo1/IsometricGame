using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerContext PlayerContext;

    private Dictionary<MovementState, State> MovementStates = new Dictionary<MovementState, State>();

    private State CurrentMovementState;

    private void Awake()
    {
        //Add the Player's movement states to the dictionary
        MovementStates.Add(MovementState.Aim, new AimState(PlayerContext));
        MovementStates.Add(MovementState.Run, new RunState(PlayerContext));
        MovementStates.Add(MovementState.Sprint, new SprintState(PlayerContext));
        MovementStates.Add(MovementState.Roll, new RollState(PlayerContext));
        MovementStates.Add(MovementState.Jump, new JumpState(PlayerContext));
        MovementStates.Add(MovementState.Fall, new FallState(PlayerContext));

        //Assigning the default state to current
        SwitchMovementState(MovementState.Aim);
    }

    public void SwitchMovementState(MovementState newState)
    {
        State state = MovementStates[newState];
        CurrentMovementState?.Exit();
        CurrentMovementState = state;
        PlayerContext.MovementState = newState;
        CurrentMovementState.Enter();
    }

    private void Update()
    {
        CurrentMovementState.Update();
    }
}
