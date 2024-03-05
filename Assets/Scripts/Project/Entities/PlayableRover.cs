using Project.Interactables;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Entities
{
    public class PlayableRover : PlayableEntity
    {
        [Header("Settings")]
        [SerializeField] private float _linearSpeed = 1.0f;
        [SerializeField] private float _angularSpeed = 1.0f;

        [Header("References")] 
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private RoverLeg[] _legs;
        [SerializeField] private Transform _interactionCheck;
        
        private Vector2 _inputBuffer;
        private bool _inputBufferInteraction = false;

        public override void Initialize()
        {
            base.Initialize();
            for (int li = 0; li < _legs.Length; li++)
            {
                _legs[li].Initialize();
            }
        }

        public void Update()
        {
            if (!this.IsActive) { return; }
            
            _inputBuffer = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

            _inputBufferInteraction |= Input.GetKeyDown(KeyCode.E);
        }

        public void FixedUpdate()
        {
            if (!this.IsActive) { return; }
            
            ApplyMovementForce();
            ApplyMovementTorque();
            ApplyLegForces();
            HandleInteraction();
            
            
            // Reset input buffers now that they've been used to avoid double counting them.
            
            _inputBuffer = Vector2.zero;
            _inputBufferInteraction = false;
        }
        
        private void ApplyMovementForce()
        {
            float input = _inputBuffer.y;
            float magnitude = input * _linearSpeed;
            var force = this.transform.forward * magnitude;
            _rb.AddForce(force, ForceMode.Force);
        }

        private void ApplyMovementTorque()
        {
            float input = _inputBuffer.x;
            var magnitude = input * _angularSpeed;
            var torque = this.transform.up * magnitude;
            _rb.AddTorque(torque, ForceMode.Force);
        }

        private void ApplyLegForces()
        {
            for (int li = 0; li < _legs.Length; li++)
            {
                _legs[li].ApplyForceToRover(_rb);
            }
            
            for (int li = 0; li < _legs.Length; li++)
            {
                _legs[li].UpdateLegPosition(_rb.velocity);
            }
        }

        private void HandleInteraction()
        {
            if (_inputBufferInteraction)
            {
                TryInteract();
            }
        }

        private void TryInteract()
        {
            var origin = _interactionCheck.position;
            var radius = 0.5f;
            var mask = LayerMask.GetMask("Interactable");

            var colliders = Physics.OverlapSphere(origin, radius, mask, QueryTriggerInteraction.Ignore);

            if (colliders.Length > 0)
            {
                var collider = colliders[0];
                var interactable = collider.gameObject.GetComponentInParent<Interactable>();

                if (interactable.TryBeginInteraction())
                {
                    // TODO...
                }
            }
        }
        
    }
}