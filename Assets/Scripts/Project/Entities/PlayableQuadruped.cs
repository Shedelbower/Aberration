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
            _legOrchestrator.Initialize(this, _rb);
        }

        private void Update()
        {
            _legOrchestrator.OnUpdate();

            _legOrchestrator.InputLinearMovement = InputManager.Instance.VerticalAxis;
            _legOrchestrator.InputAngularMovement = InputManager.Instance.HorizontalAxis;
        }

        private void FixedUpdate()
        {
            _legOrchestrator.OnFixedUpdate();
        }
        
        
        
        
    }
}