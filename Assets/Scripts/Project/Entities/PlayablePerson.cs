using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

namespace Project.Entities
{
    public class PlayablePerson : PlayableEntity
    {
        public Vector3 MovementForward => _yawBase.forward;
        public Vector3 MovementRight => _yawBase.right;
        public Vector3 RotationAxis => this.transform.up;


        [Header("Settings")]
        [SerializeField] private float _walkSpeed = 600f;
        [SerializeField] private float _sprintSpeed = 1200f;
        [SerializeField] private float _mouseSensitivityHorizontal = 800f;
        [SerializeField] private float _mouseSensitivityVertical = 800f;
        
        [SerializeField] private float _minVerticalAngleDegrees = 70f;
        [SerializeField] private float _maxVerticalAngleDegress = 70f;
        
        [Header("Component References")]
        [SerializeField] private Rigidbody _rb;
        
        [SerializeField] private Transform _yawBase;
        [SerializeField] private Transform _pitchBase;
        
        [Header("Debug")]
        [SerializeField] private bool _isGrounded;
        
        private float _desiredHeadHeight = 1.75f;

        public override void OnActivated()
        {
            base.OnActivated();
            InputManager.Instance.LockMouse();
        }
        
        public override void OnDeactivated()
        {
            base.OnDeactivated();
            InputManager.Instance.UnlockMouse();
        }

        private void Update()
        {
            // Rotation (Yaw)
            float mouseX = InputManager.Instance.MouseX;
            float angleX = mouseX * Time.deltaTime * _mouseSensitivityHorizontal;
            var rotX = Quaternion.AngleAxis(angleX, this.RotationAxis);

            _yawBase.rotation = rotX * _yawBase.rotation;
            
            // Rotation (Pitch)
            
            float mouseY = InputManager.Instance.MouseY;
            float verticalDelta = -mouseY * _mouseSensitivityVertical * Time.deltaTime;
            float verticalAngle = _pitchBase.localEulerAngles.x;

            while (verticalAngle > 90f)
            {
                verticalAngle -= 180f;
            }
            
            verticalAngle = Mathf.Clamp(verticalAngle + verticalDelta, -_minVerticalAngleDegrees, _maxVerticalAngleDegress);
            _pitchBase.localRotation = Quaternion.Euler(verticalAngle, 0, 0);
        }


        private void FixedUpdate()
        {
            if (!this.IsActive) { return; }

            GroundCheckAndGravity();
            HandleMovement();
        }

        private void GroundCheckAndGravity()
        {
            float bufferLength = 0.1f;
            float castLength = bufferLength + _desiredHeadHeight;
            var mask = LayerMask.GetMask("Walkable");

            float gravityScale = 1.0f;
            var currHeight = 10000f;
            
            if (Physics.Raycast(_pitchBase.position, Vector3.down, out RaycastHit hit, castLength, mask, QueryTriggerInteraction.Ignore))
            {
                currHeight = hit.distance;
            }
            
            if (currHeight > _desiredHeadHeight)
            {
                _isGrounded = false;
                float falloff = (currHeight - _desiredHeadHeight) / bufferLength;
                falloff = Mathf.Clamp01(falloff);
                gravityScale = falloff;
            }
            else
            {
                _isGrounded = true;
                var verticalDelta = _desiredHeadHeight - currHeight;
                _rb.MovePosition(_rb.position + Vector3.up * verticalDelta);
                var vel = _rb.velocity;
                vel.y = 0.0f;
                _rb.velocity = vel;
            }

            if (!_isGrounded)
            {
                // Gravity
                _rb.AddForce(Vector3.up * -9.8f * gravityScale, ForceMode.Acceleration); 
            }
        }

        private void HandleMovement()
        {
            if (_isGrounded)
            {
                // Movement
                var forwardInput = InputManager.Instance.VerticalAxis;
                var rightInput = InputManager.Instance.HorizontalAxis;

                var moveDir = forwardInput * this.MovementForward + rightInput * this.MovementRight;
                moveDir = moveDir.normalized;
                var speed = InputManager.Instance.ShiftKey ? _sprintSpeed : _walkSpeed;

                var newHorizontalVelocity = moveDir * speed;
                newHorizontalVelocity.y = _rb.velocity.y; // Preserve vertical velocity

                _rb.velocity = newHorizontalVelocity;
            }
            
        }

    }
}