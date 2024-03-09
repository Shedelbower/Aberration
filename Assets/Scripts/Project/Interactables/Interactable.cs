using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Project.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        [Flags] public enum EntityMask
        {
            None   = 0,
            Person  = 1 << 0,
            Quadruped = 1 << 1
        }
        
        public delegate void InteractionCallback(Interactable interactable);
        public event InteractionCallback OnInteractionCompleted;

        public Transform InteractPoint => (_interactPoint == null ? this.transform : _interactPoint);
        
        [SerializeField] private Transform _interactPoint;
        [SerializeField] protected EntityMask _entityMask = EntityMask.Person;

        private bool _initialized = false;
        private bool _isShowingUI = false;
        private int _interactionFrameCountdown = 0;

        protected virtual void Awake()
        {
            if (!_initialized)
            {
                Initialize();
            }
        }

        protected virtual void Initialize() { _initialized = true; }

        public virtual void CancelInteraction() { }

        public virtual void BroadcastInteractionCompleted()
        {
            this.OnInteractionCompleted?.Invoke(this);
        }
        
        public abstract bool TryBeginInteraction();

        public bool CanInteractWith(EntityMask mask)
        {
            return (mask & _entityMask) > 0;
        }
        
    }
}