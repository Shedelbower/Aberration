using UnityEngine;

namespace Project.Interactables
{
    public abstract class PoweredComponent : MonoBehaviour, ISignalReceiver
    {
        [SerializeField] protected SignalNetwork _powerNetwork;
        
        public bool IsPowered => _powerNetwork.SignalValue > 0;

        private bool _initialized = false;

        protected virtual void Awake()
        {
            if (!_initialized)
            {
                Initialize();
            }
        }
        
        protected virtual void Initialize()
        {
            _initialized = true;
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
        
    }
}