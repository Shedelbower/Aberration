using UnityEngine;

namespace Project.Entities
{
    public class PlayableQuadruped : PlayableEntity
    {
        [Header("Settings")]
        [SerializeField] private float _linearSpeed = 20.0f;
        [SerializeField] private float _angularSpeed = 10.0f;
        [SerializeField] private float _legPowerScale = 20.0f;
        
        [Header("Component References")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private QuadrupedLegOrchestrator _legOrchestrator;
        
        
        // Input buffers between Update and FixedUpdate
        private Vector2 _inputBufferMovement;
        private bool _inputBufferInteraction;
        
        
        public override void Initialize()
        {
            base.Initialize();
            _legOrchestrator.Initialize(_rb);
        }

        private void ResetPose()
        {
            // TODO
        }
        
        public override void OnUpdate()
        {
            // _inputBufferMovement = new Vector2(
            //     Input.GetAxisRaw("Horizontal"),
            //     Input.GetAxisRaw("Vertical")
            // );
            //
            // _inputBufferInteraction |= Input.GetKeyDown(KeyCode.E);

            _legOrchestrator.InputLinearMovement = Input.GetAxisRaw("Vertical");
            _legOrchestrator.InputAngularMovement = Input.GetAxisRaw("Horizontal");
        }

        public override void OnFixedUpdate()
        {
            // var movementForce = this.transform.forward * _inputBufferMovement.y * _linearSpeed;
            // var torque = this.transform.up * _inputBufferMovement.x * _angularSpeed;

            _legOrchestrator.OnFixedUpdate();
            
            // _legOrchestrator.ApplyForces(_rb, movementForce, torque, _legPowerScale);
            
            
            // Reset input buffers now that they've been used to avoid double counting them.
            // _inputBufferMovement = Vector2.zero;
            // _inputBufferInteraction = false;
        }
        
        
        
        
    }
}