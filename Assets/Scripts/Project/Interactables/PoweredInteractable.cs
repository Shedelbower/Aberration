using UnityEngine;

namespace Project.Interactables
{
    public abstract class PoweredInteractable : Interactable, ISignalReceiver
    {
        [SerializeField] protected SignalNetwork _powerNetwork;

        public bool IsPowered => _powerNetwork.SignalValue > 0;

        protected override void Initialize()
        {
            base.Initialize();
            _powerNetwork.RegisterReceiver(this);
        }

        public void OnSignalValueChanged(int value)
        {
            if (value <= 0)
            {
                this.OnPoweredDown();
            }
            else
            {
                this.OnPoweredUp();
            }
        }

        public void SetInitialSignalValue(int value)
        {
            SetInitialPoweredState(value > 0);
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