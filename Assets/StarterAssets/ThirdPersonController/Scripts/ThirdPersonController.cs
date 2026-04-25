 using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("Roll speed of the character in m/s")]
        public float RollSpeed = 6f;

        [Tooltip("Roll time of the character in s")]
        public float RollTime = 1f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        public float mouseIdleDelay = 3f;

        [SerializeField]
        PlayerContext PlayerContext;

        // player
        public float directionRotationSpeed = 8f;
        private float _animationBlend;
        private float _terminalVelocity = 53.0f;

        //mouse position
        private Vector2 previousMousePostion;
        private bool isMouseIdle = false;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _mouseIdleDelta;

        // Speed
        public float targetSpeed = 0f;
        private const float _threshold = 0.01f;
        private float _speed;

        // Velocity
        float _verticalVelocity = 0;

        // roll values
        private Vector3? _rollVector = null;
        private float _remainingRollTime = 0f;

        // Input Values
        Vector3 inputDirection = Vector3.zero;
        Vector2 mousePosition = Vector3.zero;

        // Calculated values
        Vector3 moveDirection = Vector3.zero;
        public Vector3 animationDirection = Vector3.zero;

        private void Start()
        {
            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            HandleAimState();
            HandleInput();
            ResolveStates();
            GroundedCheck();
            JumpAndGravity();
            Move();
        }

        private void HandleInput()
        {

            // Move direction
            inputDirection = new Vector3(PlayerContext.Input.move.x, 0.0f, PlayerContext.Input.move.y).normalized;

            // Mouse position
            mousePosition = PlayerContext.Input.look;

            // Switch to Roll state if its not already roll, jump, fall
            if(PlayerContext.Input.roll && PlayerContext.MovementState != MovementState.Roll && PlayerContext.MovementState != MovementState.Jump && PlayerContext.MovementState != MovementState.Fall)
            {
                PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Roll);
                PlayerContext.Input.roll = false;
            }

            // Switch to Jump state if its not already jump, roll, fall
            if (PlayerContext.Input.jump && PlayerContext.MovementState != MovementState.Roll && PlayerContext.MovementState != MovementState.Jump && PlayerContext.MovementState != MovementState.Fall)
            {
                PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Jump);
                PlayerContext.Input.jump = false;
            }

            // Start Sprint state if its not already sprint, jump, roll, fall, and the player is moving
            if (PlayerContext.Input.sprint && PlayerContext.MovementState != MovementState.Sprint && PlayerContext.MovementState != MovementState.Roll && PlayerContext.MovementState != MovementState.Jump && PlayerContext.MovementState != MovementState.Fall && PlayerContext.IsMoving)
            {
                PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Sprint);
            }
            else if (!PlayerContext.Input.sprint && PlayerContext.MovementState == MovementState.Sprint && PlayerContext.IsMoving)
            {
                if(isMouseIdle)
                {
                    PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Run);
                }
                else
                {
                    PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Aim);
                }
            }
        }

        private void ResolveStates()
        {
            // Switch state from sprint if the player stops moving
            if(PlayerContext.MovementState == MovementState.Sprint && !PlayerContext.IsMoving)
            {
                if (isMouseIdle)
                {
                    PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Run);
                }
                else
                {
                    PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Aim);
                }
            }

            // Switch state from aim to run if the mouse is idle
            if(isMouseIdle && PlayerContext.MovementState != MovementState.Run && PlayerContext.MovementState == MovementState.Aim)
            {
                PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Run);
            }

            // Switch state from run to aim if the mouse is not idle
            if (!isMouseIdle && PlayerContext.MovementState == MovementState.Run && PlayerContext.MovementState != MovementState.Aim)
            {
                PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Aim);
            }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            PlayerContext.Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void Move()
        {
            // move the player
            PlayerContext.Controller.Move(moveDirection * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (PlayerContext.Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                //PlayerContext.Animator.SetBool(_animIDJump, false);
                //PlayerContext.Animator.SetBool(_animIDFreeFall, false);

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (PlayerContext.Input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);


                    //PlayerContext.Animator.SetBool(_animIDJump, true);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {

                    //PlayerContext.Animator.SetBool(_animIDFreeFall, true);
                }

                // if we are not grounded, do not jump
                PlayerContext.Input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (PlayerContext.Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        /*private void Roll()
        {
            if(!_rollVector.HasValue)
            {
                PlayerContext.IsRolling = true;
                _rollVector = PlayerContext.MoveDirection;
                _remainingRollTime = RollTime;
                PlayerContext.Animator.SetBool(_animIdIsRolling, true);
                ParentConstraint parentConstraint = PlayerContext.Guns[0].main.GetComponentInChildren<ParentConstraint>();
                TwoBoneIKConstraint[] twoBoneIKConstraints = PlayerContext.Rig.GetComponentsInChildren<TwoBoneIKConstraint>();
                parentConstraint.weight = 1;
                twoBoneIKConstraints[0].weight = 0;
                twoBoneIKConstraints[1].weight = 0;
                parentConstraint.constraintActive = true;
            }

            ApplyDirectionRotation(_rollVector.Value, 10);
            _speed = GetSpeed(RollSpeed);
            if (_remainingRollTime < 0.1f)
            {
                _speed = GetSpeed(MoveSpeed - 2);
            }
            PlayerContext.Controller.Move(_rollVector.Value * (_speed * Time.deltaTime) +
                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            _remainingRollTime -= Time.deltaTime;



            if(_remainingRollTime < 0.0f)
            {
                _rollVector = null;
                PlayerContext.IsRolling = false;
                PlayerContext.Input.roll = false;
                PlayerContext.Animator.SetBool(_animIdIsRolling, false);
                ParentConstraint parentConstraint = PlayerContext.Guns[0].main.GetComponentInChildren<ParentConstraint>();
                TwoBoneIKConstraint[] twoBoneIKConstraints = PlayerContext.Rig.GetComponentsInChildren<TwoBoneIKConstraint>();
                parentConstraint.weight = 0;
                twoBoneIKConstraints[0].weight = 1;
                twoBoneIKConstraints[1].weight = 1;
                parentConstraint.constraintActive = false;
                _speed = MoveSpeed;
            }
        }*/

        private float GetSpeed(float targetSpeed)
        {
            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(PlayerContext.Controller.velocity.x, 0.0f, PlayerContext.Controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = PlayerContext.Input.move.magnitude;

            float speed = 0f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }

            return speed;
        }

        public void ApplyAimRotation()
        {
            Quaternion targetRotation = CalculateAimRotation();
            transform.rotation = targetRotation;
        }

        public void ApplyDirectionRotation(float speed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0.0f, moveDirection.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }

        private Quaternion CalculateAimRotation()
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray mousePointRay = PlayerContext.MainCamera.ScreenPointToRay(mousePosition);

            if (groundPlane.Raycast(mousePointRay, out float hit))
            {

                Vector3 hitPoint = mousePointRay.GetPoint(hit);
                Vector3 direction = hitPoint - transform.position;

                if (direction.sqrMagnitude > 0.01f)
                {
                    return Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z));
                }
            }

            return transform.rotation;
        }

        public void HandleTargetSpeed(float speed)
        {
            targetSpeed = speed;

            if (PlayerContext.Input.move == Vector2.zero)
            {
                PlayerContext.IsMoving = false;
                targetSpeed = 0.0f;
            }
            else
            {
                PlayerContext.IsMoving = true;
            }
        }

        private void HandleAimState()
        {
            if (previousMousePostion == mousePosition)
            {
                _mouseIdleDelta -= Time.deltaTime;
            }
            else
            {
                _mouseIdleDelta = mouseIdleDelay;
                isMouseIdle = false;
            }
            previousMousePostion = mousePosition;
            if (_mouseIdleDelta <= 0f)
            {
                isMouseIdle = true;
            }
        }

        public void SetupCharacterDirection()
        {
            _speed = GetSpeed(targetSpeed);

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            if (inputDirection == Vector3.zero)
            {
                moveDirection = transform.forward;
                animationDirection = Vector3.zero;
            }
            else
            {
                moveDirection = Quaternion.Euler(0, 45, 0) * inputDirection;
                animationDirection = transform.InverseTransformDirection(moveDirection);
            }
        }
    }
}