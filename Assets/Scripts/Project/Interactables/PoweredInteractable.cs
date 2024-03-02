using UnityEngine;

namespace Project.Interactables
{
    public abstract class PoweredInteractable : Interactable, IPowered
    {
        private bool _isPowered;

        public virtual void SetInitialPoweredState(bool isPowered)
        {
            _isPowered = isPowered;
        }
        
        public virtual void OnPoweredUp()
        {
            _isPowered = true;
        }
        
        public virtual void OnPoweredDown()
        {
            _isPowered = false;
        }
        
        
        public override bool TryBeginInteraction()
        {
            return _isPowered;
        }
    }
}