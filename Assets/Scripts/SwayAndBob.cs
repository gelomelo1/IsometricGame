
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class SwayNBobScript : MonoBehaviour
{
    public PlayerContext PlayerContext;
    [Header("Sway")]
    public float Step = 0.01f;
    public float MaxStepDistance = 0.06f;
    Vector3 SwayPos;

    [Header("Sway Rotation")]
    public float RotationStep = 4f;
    public float MaxRotationStep = 5f;
    Vector3 SwayEulerRot;

    public float Smooth = 10f;
    float SmoothRot = 12f;

    [Header("Bobbing")]
    public float SpeedCurve;
    float CurveSin { get => Mathf.Sin(SpeedCurve); }
    float CurveCos { get => Mathf.Cos(SpeedCurve); }

    public Vector3 TravelLimit = Vector3.one * 0.025f;
    public Vector3 BobLimit = Vector3.one * 0.01f;
    public Vector3 TravelLimitZoomAim = Vector3.one * 0.025f;
    public Vector3 BobLimitZoomAim = Vector3.one * 0.01f;
    Vector3 BobPosition;

    public float BobExaggeration = 2;
    public float BobExaggerationIdle = 1;
    public float BobExaggerationSprint = 4;
    public float BobExaggerationZoomAim = 3;

    [Header("Bob Rotation")]
    public Vector3 Multiplier;
    public Vector3 MultiplierZoomAim;
    Vector3 BobEulerRotation;

    private Vector3 CurrentTravelLimit;
    private Vector3 CurrentBobLimit;
    private float CurrentExaggeration;
    private Vector3 CurrentMultiplier;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {

        if (PlayerContext.IsReloading)
            return;

        GetInput();
        CalculateBobSwayParams();

        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();

        CompositePositionRotation();
    }


    Vector2 walkInput;
    Vector2 lookInput;

    void GetInput()
    {
        walkInput = PlayerContext.Input.move.normalized;

        lookInput = PlayerContext.Input.look.normalized;
    }


    void Sway()
    {
        Vector3 invertLook = lookInput * -Step;
        invertLook.x = Mathf.Clamp(invertLook.x, -MaxStepDistance, MaxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -MaxStepDistance, MaxStepDistance);

        SwayPos = invertLook;
    }

    void SwayRotation()
    {
        Vector2 invertLook = lookInput * -RotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -MaxRotationStep, MaxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -MaxRotationStep, MaxRotationStep);
        SwayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void CompositePositionRotation()
    {
        Vector3 targetPosition = initialPosition + SwayPos + BobPosition;

        Quaternion targetRotation =
            initialRotation *
            Quaternion.Euler(SwayEulerRot) *
            Quaternion.Euler(BobEulerRotation);

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * Smooth
        );

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * SmoothRot
        );
    }

    void BobOffset()
    {

        // Calculate the total input magnitude to handle diagonal movement properly
        float inputMagnitude = walkInput.magnitude;


        Vector3 limit = CurrentBobLimit * CurrentExaggeration;
        // Now use the total input magnitude for smooth bobbing
        SpeedCurve += Time.deltaTime * (PlayerContext.Grounded ? inputMagnitude * CurrentExaggeration : BobExaggerationIdle);
        BobPosition.x = (Mathf.Cos(SpeedCurve) * limit.x) - (inputMagnitude * CurrentTravelLimit.x);  // Use inputMagnitude here
        BobPosition.y = (Mathf.Sin(SpeedCurve * 2) * limit.y) - (inputMagnitude * CurrentTravelLimit.y);  // Use inputMagnitude here

    }

    void BobRotation()
    {
        BobEulerRotation.x = (walkInput != Vector2.zero ? CurrentMultiplier.x * (Mathf.Sin(2 * SpeedCurve)) : CurrentMultiplier.x * (Mathf.Sin(2 * SpeedCurve) / 2));
        BobEulerRotation.y = (walkInput != Vector2.zero ? CurrentMultiplier.y * CurveCos : 0);
        BobEulerRotation.z = (walkInput != Vector2.zero ? CurrentMultiplier.z * CurveCos * walkInput.x : 0);
    }

    private void CalculateBobSwayParams()
    {
        CurrentExaggeration = BobExaggeration;
        CurrentTravelLimit = TravelLimit;
        CurrentBobLimit = BobLimit;
        CurrentMultiplier = Multiplier;

        // Calculate the total input magnitude to handle diagonal movement properly
        float inputMagnitude = walkInput.magnitude;
        if (PlayerContext.MovementState == MovementState.Aim)
        {
            CurrentExaggeration = BobExaggerationZoomAim;
            CurrentTravelLimit = TravelLimitZoomAim;
            CurrentBobLimit = BobLimitZoomAim;
            CurrentMultiplier = MultiplierZoomAim;
        }
        else if (inputMagnitude > 0.1f)
        {
            if (PlayerContext.MovementState == MovementState.Sprint)
            {
                CurrentExaggeration = BobExaggerationSprint;
            }
        }
        else
        {
            CurrentExaggeration = BobExaggerationIdle;
        }
    }

}