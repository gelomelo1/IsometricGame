using StarterAssets;
using UnityEngine;

public class GunTransform : MonoBehaviour
{
    [SerializeField]
    PlayerContext PlayerContext;

    [SerializeField]
    Transform Idle;

    [SerializeField]
    Transform Run;

    [SerializeField]
    Transform Aim;

    [SerializeField]
    private float SmoothTime = 0.2f;

    [SerializeField]
    private float SmoothTimeAim = 0.05f;

    private Transform TargetTransform;
    private GunTransformState CurrentGunTransformState = GunTransformState.Idle;
    private Vector3 Velocity = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TargetTransform = Idle;
        copyTransform(true);
    }

    // Update is called once per frame
    void Update()
    {

            if(IsIdleMoving())
        {
            SetState(GunTransformState.Run);
        }
        else if (PlayerContext.MovementState == MovementState.Aim)
        {
            SetState(GunTransformState.Aim);
        }
        else
        {
            SetState(GunTransformState.Idle);
        }

        copyTransform();
    }

    private void copyTransform(bool instant = false)
    {
        if (instant)
        {
            transform.localPosition = TargetTransform.localPosition;
            transform.localRotation = TargetTransform.localRotation;
        }
        else
        {
            float selectedSmoothTime;
            switch (CurrentGunTransformState)
            {
                case GunTransformState.Aim:
                    selectedSmoothTime = SmoothTimeAim;
                    break;
                default:
                    selectedSmoothTime = SmoothTime;
                    break;
            }
            transform.localPosition = Vector3.SmoothDamp(
                transform.localPosition,
                TargetTransform.localPosition,
                ref Velocity,
                selectedSmoothTime
            );


            transform.localRotation = Quaternion.Slerp(
    transform.localRotation,
    TargetTransform.localRotation,
    1f - Mathf.Exp(-Time.deltaTime / selectedSmoothTime)
);
        }
    }

    private void SetState(GunTransformState newState)
    {
        if (newState == CurrentGunTransformState)
            return;

        CurrentGunTransformState = newState;

        switch (newState)
        {
            case GunTransformState.Run:
                TargetTransform = Run;
                break;
            case GunTransformState.Aim:
                TargetTransform = Aim;
                break;
            default:
                TargetTransform = Idle;
                break;
        }
    }

    private bool IsIdleMoving()
    {
        if(PlayerContext.IsMoving && PlayerContext.MovementState == MovementState.Run)
        {
            return true;
        }
        return false;
    }
}
