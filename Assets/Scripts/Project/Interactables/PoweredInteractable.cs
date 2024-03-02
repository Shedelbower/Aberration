using UnityEngine;

namespace Project.Interactables
{
    public abstract class PoweredInteractable : Interactable
    {
        [SerializeField] protected SignalNetwork<bool> _powerNetwork;

        public bool IsPowered => _powerNetwork.SignalValue;
        
        protected override void Initialize()
        {
            base.Initialize();
            _powerNetwork.OnSignalChanged += OnSignalValueChanged;
            SetInitialPoweredState(_powerNetwork.SignalValue);
        }

        public void OnSignalValueChanged(bool value)
        {
            if (value)
            {
                this.OnPoweredUp();
            }
            else
            {
                this.OnPoweredDown();
            }
        }
        
        protected abstract void SetInitialPoweredState(bool isPowered);
        protected abstract void OnPoweredUp();
        protected abstract void OnPoweredDown();
        
        
        public override bool TryBeginInteraction()
        {
            // Can only interact when powered
            return this.IsPowered;
        }
    }
}