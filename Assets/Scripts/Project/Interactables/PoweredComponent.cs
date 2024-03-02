using UnityEngine;

namespace Project.Interactables
{
    public abstract class PoweredComponent : MonoBehaviour
    {
        [SerializeField] protected SignalNetwork<bool> _powerNetwork;
        
        public bool IsPowered => _powerNetwork.SignalValue;

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
        
    }
}