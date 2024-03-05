using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

namespace Project.Entities
{
    public class PlayablePerson : PlayableEntity
    {
        public Vector3 MovementForward => this.transform.forward;
        public Vector3 MovementRight => this.transform.right;
        public Vector3 RotationAxis => this.transform.up;


        [Header("Settings")]
        [SerializeField] private float _walkSpeed = 600f;
        [SerializeField] private float _sprintSpeed = 1200f;
        [SerializeField] private float _mouseSensitivityHorizontal = 800f;
        [SerializeField] private float _mouseSensitivityVertical = 800f;
        
        [SerializeField] private float _minVerticalAngleDegrees = 70f;
        [SerializeField] private float _maxVerticalAngleDegress = 70f;
        
        [Header("Component References")]
        [SerializeField] private CharacterController _controller;

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
            if (!this.IsActive) { return; }

            HandleMovement();
        }

        private void HandleMovement()
        {
            // Movement
            var forwardInput = InputManager.Instance.VerticalAxis;
            var rightInput = InputManager.Instance.HorizontalAxis;

            var moveDir = forwardInput * this.MovementForward + rightInput * this.MovementRight;
            moveDir = moveDir.normalized;
            var speed = InputManager.Instance.ShiftKey ? _sprintSpeed : _walkSpeed;

            _controller.SimpleMove(moveDir * speed * Time.deltaTime);
            
            // Rotation
            float mouseX = InputManager.Instance.MouseX;
            float angleX = mouseX * Time.deltaTime * _mouseSensitivityHorizontal;
            var rotX = Quaternion.AngleAxis(angleX, this.RotationAxis);

            this.transform.rotation = rotX * this.transform.rotation;
            
            float mouseY = InputManager.Instance.MouseY;
            float verticalDelta = -mouseY * _mouseSensitivityVertical * Time.deltaTime;
            float verticalAngle = _camera.transform.localEulerAngles.x;

            while (verticalAngle > 90f)
            {
                verticalAngle -= 180f;
            }
            
            verticalAngle = Mathf.Clamp(verticalAngle + verticalDelta, -_minVerticalAngleDegrees, _maxVerticalAngleDegress);
            _camera.transform.localRotation = Quaternion.Euler(verticalAngle, 0, 0);
        }

    }
}