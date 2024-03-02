using UnityEngine;

namespace Project.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        public delegate void InteractionCallback(Interactable interactable);
        public event InteractionCallback OnInteractionCompleted;

        private bool _initialized = false;

        protected virtual void Awake()
        {
            if (!_initialized)
            {
                Initialize();
            }
        }
        
        protected virtual void Initialize() { }

        public virtual void CancelInteraction() { }

        public virtual void BroadcastInteractionCompleted()
        {
            this.OnInteractionCompleted?.Invoke(this);
        }
        
        public abstract bool TryBeginInteraction();
        
    }
}