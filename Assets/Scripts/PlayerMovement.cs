using StarterAssets;
using UnityEngine;
using UnityEngine.EventSystems;


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

        float inputMagnitude = PlayerContext.Input.move.magnitude;

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

        PlayerContext.Animator.SetFloat("Speed", PlayerContext.Input.move.magnitude, 0.1f, Time.deltaTime);
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

        PlayerContext.Animator.SetFloat("Speed", PlayerContext.Input.move.magnitude, 0.1f, Time.deltaTime);
    }
}

public class RollState : State
{
    public RollState(PlayerContext playerContext) : base(playerContext)
    {
    }
}

public class JumpState : State
{
    public JumpState(PlayerContext playerContext) : base(playerContext)
    {
    }
}

public class FallState : State
{
    public FallState(PlayerContext playerContext) : base(playerContext)
    {
    }
}
