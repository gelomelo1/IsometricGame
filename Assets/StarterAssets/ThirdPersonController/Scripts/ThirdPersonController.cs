using System.Collections;
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
        [Tooltip("Time required to pass before being able to do movement action again.")]
        public float MovementActionTimeout = 0.5f;

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
        public float _movementActionTimeoutDelta = 0f;
        private float _mouseIdleDelta;

        // Speed
        public float targetSpeed = 0f;
        private const float _threshold = 0.01f;
        public float _speed;

        // Velocity
        float _verticalVelocity = 0;

        // roll values
        public Vector3? _rollVector = null;
        public float _remainingRollTime = 0f;

        // Input Values
        Vector3 inputDirection = Vector3.zero;
        Vector2 mousePosition = Vector3.zero;

        // Calculated values
        public Vector3 moveDirection = Vector3.zero;
        public Vector3 animationDirection = Vector3.zero;

        private void Update()
        {
            HandleMovementActionTimeout();
            HandleAimState();
            HandleInput();
            ResolveStates();
            GroundedCheck();
            ApplyGravity();
            Move();
        }

        private void HandleInput()
        {

            // Move direction
            inputDirection = new Vector3(PlayerContext.Input.move.x, 0.0f, PlayerContext.Input.move.y).normalized;

            // Mouse position
            mousePosition = PlayerContext.Input.look;

            // Switch to Roll state if timeout expired and its not already roll, jump, fall
            if(PlayerContext.Input.roll && PlayerContext.MovementState != MovementState.Roll && PlayerContext.MovementState != MovementState.Jump && PlayerContext.MovementState != MovementState.Fall)
            {
                if(_movementActionTimeoutDelta < 0)
                {
                    PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Roll);
                }
                PlayerContext.Input.roll = false;
            }

            // Switch to Jump state if timeout expired and its not already jump, roll, fall
            if (PlayerContext.Input.jump && PlayerContext.MovementState != MovementState.Roll && PlayerContext.MovementState != MovementState.Jump && PlayerContext.MovementState != MovementState.Fall)
            {
                if(_movementActionTimeoutDelta < 0)
                {
                    PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Jump);
                }
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
            if(isMouseIdle &&  PlayerContext.MovementState == MovementState.Aim)
            {
                PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Run);
            }

            // Switch state from run to aim if the mouse is not idle
            if (!isMouseIdle && PlayerContext.MovementState == MovementState.Run)
            {
                PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Aim);
            }

            // Switch state from jump to fall if velocity becomes negative and player not grounded
            if(!PlayerContext.Grounded && _verticalVelocity < 0 && PlayerContext.MovementState == MovementState.Jump)
            {
                PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Fall);
            }

            // Switch state from other state to fall if player not grounded, and not in jumping state
            if (!PlayerContext.Grounded && PlayerContext.MovementState != MovementState.Jump && PlayerContext.MovementState != MovementState.Fall)
            {
                PlayerContext.PlayerStateMachine.SwitchMovementState(MovementState.Fall);
            }

            // Switch state from fall to other state if player become grounded
            if (PlayerContext.Grounded && PlayerContext.MovementState == MovementState.Fall)
            {
                MovementState newMovement = DetermineNextState();
                PlayerContext.PlayerStateMachine.SwitchMovementState(newMovement);
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

        private void ApplyGravity()
        {
            if (PlayerContext.Grounded && _verticalVelocity < 0.0f)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }

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

        public void Roll()
        {
            ApplyDirectionRotation(10, _rollVector.Value);
            _speed = GetSpeed(RollSpeed);
            if (_remainingRollTime < 0.1f)
            {
                _speed = GetSpeed(MoveSpeed - 2);
            }
            _remainingRollTime -= Time.deltaTime;
            if(_remainingRollTime < 0.0f)
            {
                MovementState newMovement = DetermineNextState();
                PlayerContext.PlayerStateMachine.SwitchMovementState(newMovement);
            }
        }

        public void Jump()
        {
            StartCoroutine(JumpRoutine());
        }

        private IEnumerator JumpRoutine()
        {
            yield return new WaitForSeconds(0.1f);
            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        private float GetSpeed(float targetSpeed)
        {
            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(PlayerContext.Controller.velocity.x, 0.0f, PlayerContext.Controller.velocity.z).magnitude;

            float speedOffset = 0.1f;

            float inputMagnitude = 1;

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

        public void ApplyDirectionRotation(float speed, Vector3? direction = null)
        {
            Vector3 currentDirection;
            if(direction.HasValue)
            {
                currentDirection = direction.Value;
            }
            else
            {
                currentDirection = new Vector3(moveDirection.x, 0.0f, moveDirection.z);
            }
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
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

        private void HandleMovementActionTimeout()
        {
            if (_movementActionTimeoutDelta >= 0)
            {
                _movementActionTimeoutDelta -= Time.deltaTime;
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

        private MovementState DetermineNextState()
        {
            MovementState newMovement = MovementState.Run;
            if (!PlayerContext.Grounded)
            {
                newMovement = MovementState.Fall;
            }
            else if (PlayerContext.Input.sprint)
            {
                newMovement = MovementState.Sprint;
            }
            else if (!isMouseIdle)
            {
                newMovement = MovementState.Aim;
            }

            return newMovement;
        }
    }
}