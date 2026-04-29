using StarterAssets;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using System.Collections;


public class AimState : State
{
    private ThirdPersonController PlayerController;
    public AimState(PlayerContext playerContext) : base(playerContext)
    {
        PlayerController = PlayerContext.PlayerController;
    }

    public override void Enter()
    {
        PlayerContext.Animator.SetInteger("MovementState", 0);
    }

    public override void Update()
    {

        PlayerController.HandleTargetSpeed(PlayerController.MoveSpeed);
        PlayerController.SetupCharacterDirection();
        PlayerController.ApplyAimRotation();

        float inputMagnitude = 0;

        if (PlayerContext.Input.move.magnitude > 0)
        {
            inputMagnitude = 1;
        }
        

        PlayerContext.Animator.SetFloat("Vertical", PlayerController.animationDirection.z * inputMagnitude, 0.1f, Time.deltaTime);
        PlayerContext.Animator.SetFloat("Horizontal", PlayerController.animationDirection.x * inputMagnitude, 0.1f, Time.deltaTime);
    }
}

public class RunState : State
{
    private ThirdPersonController PlayerController;
    public RunState(PlayerContext playerContext) : base(playerContext)
    {
        PlayerController = PlayerContext.PlayerController;
    }

    public override void Enter()
    {
        PlayerContext.Animator.SetInteger("MovementState", 1);
    }

    public override void Update()
    {

        PlayerController.HandleTargetSpeed(PlayerController.MoveSpeed);
        PlayerController.SetupCharacterDirection();
        PlayerController.ApplyDirectionRotation(PlayerController.directionRotationSpeed);

        float inputMagnitude = 0;

        if (PlayerContext.Input.move.magnitude > 0)
        {
            inputMagnitude = 1;
        }

        PlayerContext.Animator.SetFloat("Speed", inputMagnitude, 0.1f, Time.deltaTime);
    }
}

public class SprintState : State
{
    private ThirdPersonController PlayerController;
    public SprintState(PlayerContext playerContext) : base(playerContext)
    {
        PlayerController = PlayerContext.PlayerController;
    }

    public override void Enter()
    {
        PlayerContext.Animator.SetInteger("MovementState", 2);
    }

    public override void Update()
    {

        PlayerController.HandleTargetSpeed(PlayerController.SprintSpeed);
        PlayerController.SetupCharacterDirection();
        PlayerController.ApplyDirectionRotation(PlayerController.directionRotationSpeed);

        float inputMagnitude = 0;

        if (PlayerContext.Input.move.magnitude > 0)
        {
            inputMagnitude = 1;
        }

        PlayerContext.Animator.SetFloat("Speed", inputMagnitude, 0.1f, Time.deltaTime);
    }
}

public class RollState : State
{
    private ThirdPersonController PlayerController;
    public RollState(PlayerContext playerContext) : base(playerContext)
    {
        PlayerController = PlayerContext.PlayerController;
    }

    public override void Enter()
    {
        PlayerController._rollVector = PlayerController.moveDirection;
        PlayerController._remainingRollTime = PlayerController.RollTime;
        PlayerContext.Animator.SetInteger("MovementState", 3);
        ParentConstraint parentConstraint = PlayerContext.Guns[0].main.GetComponentInChildren<ParentConstraint>();
        TwoBoneIKConstraint[] twoBoneIKConstraints = PlayerContext.Rig.GetComponentsInChildren<TwoBoneIKConstraint>();
        parentConstraint.weight = 1;
        twoBoneIKConstraints[0].weight = 0;
        twoBoneIKConstraints[1].weight = 0;
        parentConstraint.constraintActive = true;
    }

    public override void Update()
    {
        PlayerController.Roll();
    }

    public override void Exit()
    {
        ParentConstraint parentConstraint = PlayerContext.Guns[0].main.GetComponentInChildren<ParentConstraint>();
        TwoBoneIKConstraint[] twoBoneIKConstraints = PlayerContext.Rig.GetComponentsInChildren<TwoBoneIKConstraint>();
        parentConstraint.weight = 0;
        twoBoneIKConstraints[0].weight = 1;
        twoBoneIKConstraints[1].weight = 1;
        parentConstraint.constraintActive = false;
        PlayerController._speed = PlayerController.MoveSpeed;
        PlayerController._movementActionTimeoutDelta = PlayerController.MovementActionTimeout;
    }
}

public class JumpState : State
{
    private ThirdPersonController PlayerController;
    public JumpState(PlayerContext playerContext) : base(playerContext)
    {
        PlayerController = PlayerContext.PlayerController;
    }

    public override void Enter()
    {
        PlayerContext.Animator.SetInteger("MovementState", 4);
        PlayerController.Jump();
    }

    public override void Exit()
    {
        PlayerController._movementActionTimeoutDelta = PlayerController.MovementActionTimeout;
    }
}

public class FallState : State
{
    private ThirdPersonController PlayerController;
    public FallState(PlayerContext playerContext) : base(playerContext)
    {
        PlayerController = PlayerContext.PlayerController;
    }

    public override void Enter()
    {
        PlayerContext.Animator.SetInteger("MovementState", 5);
    }

    public override void Exit()
    {
        PlayerContext.Animator.SetTrigger("Land");
        PlayerController._movementActionTimeoutDelta = PlayerController.MovementActionTimeout;
    }
}
