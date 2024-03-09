using System;
using Project.Interactables;
using UnityEngine;

namespace Project.Entities
{
    public class PlayableQuadruped : PlayableEntity
    {
        private static readonly int MAX_INTERACTABLE_COLLIDER_COUNT = 3;
        
        [Header("Settings")]
        
        [Header("Component References")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private QuadrupedLegOrchestrator _legOrchestrator;
        [SerializeField] private Transform _interactionZone;
        [SerializeField] private float _maxInteractionAngle = 60f;
        
        private Collider[] _interactableColliders = new Collider[MAX_INTERACTABLE_COLLIDER_COUNT];

        private Interactable _interactableToSelect;
        
        // Input buffers between Update and FixedUpdate
        private Vector2 _inputBufferMovement;
        private bool _inputBufferInteraction;

        private bool _isReachingForInteractable;
        
        
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

            if (this.IsActive)
            {
                CheckForInteractables();
                
                TryToInteract();
            }
        }

        private void FixedUpdate()
        {
            _legOrchestrator.OnFixedUpdate();
        }

        private void CheckForInteractables()
        {
            _interactableToSelect = null;
            
            var mask = LayerMask.GetMask("Interactable");
            var center = _interactionZone.position;
            var radius = _interactionZone.localScale.x;
            int colliderCount = Physics.OverlapSphereNonAlloc(center, radius, _interactableColliders, mask);
            
            if (colliderCount >= MAX_INTERACTABLE_COLLIDER_COUNT)
            {
                Debug.LogWarning("Reached max interactable collider count for quadruped. May need to raise it?");
            }
            
            for (int ci = 0; ci < colliderCount; ci++)
            {
                var collider = _interactableColliders[ci];
                var interactable = collider.GetComponentInParent<Interactable>();
                if (!interactable.CanInteractWith(Interactable.EntityMask.Quadruped))
                {
                    continue; // Not for me :(
                }

                var zoneCenter = _interactionZone.position;
                var zoneRadius = _interactionZone.localScale.x;
                var zoneDir = _interactionZone.forward;

                var interactPosition = interactable.InteractPoint.position;
                var interactVec = (interactPosition - zoneCenter);
                var interactDist = interactVec.magnitude;
                var interactDir = interactVec / interactDist;

                if (interactDist > zoneRadius)
                {
                    continue;
                }
                
                float interactAngle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(interactDir, zoneDir));
                
                if (interactAngle > _maxInteractionAngle)
                {
                    continue;
                }
                
                // Good to interact with
                PlayerManager.Instance.ShowInteractionUIFor(interactable);
                _interactableToSelect = interactable;
                break;
            }
        }

        private void TryToInteract()
        {
            if (InputManager.Instance.LeftMouseDown)
            {
                if (_interactableToSelect != null)
                {
                    _isReachingForInteractable = true;
                    var pos = _interactableToSelect.InteractPoint.position;
                    _legOrchestrator.Arm.StartReachingForTarget(FinishedReachingForInteractable, pos);
                }
            }
        }

        private void FinishedReachingForInteractable(bool didReach)
        {
            _isReachingForInteractable = false;
            // TODO: Better handle the interactable having moved out of range during reach
            if (didReach && _interactableToSelect != null)
            {
                _interactableToSelect.TryBeginInteraction();   
            }
            _legOrchestrator.Arm.ResetArmTarget();
        }

        private void OnDrawGizmosSelected()
        {
            if (_interactionZone != null)
            {
                Gizmos.color = Color.yellow;
                float radius = _interactionZone.localScale.x;
                Gizmos.DrawWireSphere(_interactionZone.position, radius);
                Gizmos.DrawLine(_interactionZone.position, _interactionZone.position + _interactionZone.forward * radius);
            }
        }
    }
}