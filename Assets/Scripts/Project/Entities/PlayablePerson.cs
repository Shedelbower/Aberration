using Project.Interactables;
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
        [SerializeField] private float _walkSpeed = 3f;
        [SerializeField] private float _sprintSpeed = 8f;
        [SerializeField] private float _jumpPower = 3f;
        [SerializeField] private float _mouseSensitivityHorizontal = 600f;
        [SerializeField] private float _mouseSensitivityVertical = 500f;
        
        [SerializeField] private float _minVerticalAngleDegrees = 70f;
        [SerializeField] private float _maxVerticalAngleDegress = 70f;
        
        [SerializeField] private float _maxInteractionDistance = 2f;
        
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

            _camera.transform.parent = null;
        }
        
        public override void OnDeactivated()
        {
            base.OnDeactivated();
            InputManager.Instance.UnlockMouse();
            
            _camera.transform.parent = _pitchBase;
        }

        private void Update()
        {
            if (!this.IsActive) { return; }
            
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
            
            // Update Camera
            UpdateCamera();
            
            // Try interaction
            if (InputManager.Instance.LeftMouseDown)
            {
                TryInteract();
            }
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
            else
            {
                if (InputManager.Instance.SpaceDownPersist || InputManager.Instance.Space) // Jump
                {
                    InputManager.Instance.SpaceDownPersist = false; // Manually flip it to false
                    _rb.AddForce(Vector3.up * _jumpPower, ForceMode.VelocityChange); 
                }
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

        private void UpdateCamera()
        {
            var tr = _camera.transform;

            tr.rotation = _pitchBase.rotation;
            var targetVec = _pitchBase.position - tr.position;
            tr.position += targetVec * 0.1f;
        }

        private void TryInteract()
        {
            var origin = _camera.transform.position;
            var dir = _camera.transform.forward;
            var mask = LayerMask.GetMask("Interactable");
            if (Physics.Raycast(origin, dir, out RaycastHit hit, _maxInteractionDistance, mask))
            {
                var interactable = hit.collider.gameObject.GetComponentInParent<Interactable>();
                if (interactable == null)
                {
                    Debug.LogWarning("Hit a collider marked \"Interactable\" with no Interactable component");
                }
                else
                {
                    interactable.TryBeginInteraction();
                }
            }
            
        }

    }
}