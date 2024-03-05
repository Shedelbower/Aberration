using Project.Interactables;
using UnityEngine;

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

        [SerializeField] private AnimationCurve _tabletFocusAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float _tabletFocusAnimationDuration = 1.0f;
        
        [Header("Component References")]
        [SerializeField] private Rigidbody _rb;

        [SerializeField] private Transform _smoothBase;
        [SerializeField] private Transform _yawBase;
        [SerializeField] private Transform _pitchBase;
        [SerializeField] private Transform _tablet;
        
        [SerializeField] private Transform _tabletFocusedTarget;
        [SerializeField] private Transform _tabletUnFocusedTarget;
        
        [Header("Debug")]
        [SerializeField] private bool _isGrounded;
        
        private float _desiredHeadHeight = 1.75f;

        private enum TabletFocusState
        {
            Focused,
            Unfocused,
            Focusing,
            Unfocusing
        }

        private TabletFocusState _tabletFocusState;
        private float _tabletFocusTimer = 0.0f;
        

        public override void OnActivated()
        {
            base.OnActivated();
            InputManager.Instance.LockMouse();

            _smoothBase.parent = null;
            _tabletFocusState = TabletFocusState.Unfocusing;
        }
        
        public override void OnDeactivated()
        {
            base.OnDeactivated();
            InputManager.Instance.UnlockMouse();
            
            _smoothBase.parent = this.transform;
            _tabletFocusState = TabletFocusState.Focusing;
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
            UpdateSmoothBase();
            
            // Try interaction
            if (InputManager.Instance.LeftMouseDown)
            {
                TryInteract();
            }
        }


        private void FixedUpdate()
        {
            if (!this.IsActive)
            {
                UpdateSmoothBase(); // TODO: Remove
                return;
            }

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

        private void UpdateSmoothBase()
        {
            var targetVec = this.transform.position - _smoothBase.position;
            _smoothBase.position += targetVec * 0.1f;

            UpdateTablet();
        }

        private void UpdateTablet()
        {
            if (_tabletFocusState == TabletFocusState.Focusing || _tabletFocusState == TabletFocusState.Unfocusing)
            {
                _tabletFocusTimer += Time.deltaTime;
                var start = _tabletFocusState == TabletFocusState.Focusing
                    ? _tabletUnFocusedTarget
                    : _tabletFocusedTarget;
                var end = _tabletFocusState == TabletFocusState.Focusing
                    ? _tabletFocusedTarget
                    : _tabletUnFocusedTarget;
                float s = Mathf.Clamp01(_tabletFocusTimer / _tabletFocusAnimationDuration);
                var t = _tabletFocusAnimationCurve.Evaluate(s);

                _tablet.position = Vector3.LerpUnclamped(start.position, end.position, t);
                _tablet.rotation = Quaternion.LerpUnclamped(start.rotation, end.rotation, t);
                if (s >= 1f)
                {
                    if (_tabletFocusState == TabletFocusState.Unfocusing)
                    {
                        _tabletFocusState = TabletFocusState.Unfocused;
                        _tablet.parent = _tabletUnFocusedTarget;
                        _tabletFocusTimer = 0.0f;
                    }
                    else
                    {
                        _tabletFocusState = TabletFocusState.Focused;
                        _tablet.parent = _tabletFocusedTarget;
                        _tabletFocusTimer = 0.0f;
                    }
                }
            }
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