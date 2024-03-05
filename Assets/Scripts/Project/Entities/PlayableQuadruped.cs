using UnityEngine;

namespace Project.Entities
{
    public class PlayableQuadruped : PlayableEntity
    {
        [Header("Settings")]
        
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

        private void Update()
        {
            if (!this.IsActive) { return; }
            
            _legOrchestrator.OnUpdate();

            _legOrchestrator.InputLinearMovement = InputManager.Instance.VerticalAxis;
            _legOrchestrator.InputAngularMovement = InputManager.Instance.HorizontalAxis;
        }

        private void FixedUpdate()
        {
            if (!this.IsActive) { return; }
            
            _legOrchestrator.OnFixedUpdate();
        }
        
        
        
        
    }
}