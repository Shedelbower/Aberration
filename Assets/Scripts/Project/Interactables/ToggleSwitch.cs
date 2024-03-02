using UnityEngine;

namespace Project.Interactables
{
    public class ToggleSwitch : PoweredInteractable
    {
        [SerializeField] private SignalNetwork<bool> _switchNetwork;
        [SerializeField] private MeshRenderer _powerIndicatorRenderer;
        [SerializeField] private MeshRenderer _switchIndicatorRenderer;

        [SerializeField] private Material _poweredIndicatorMaterial;
        [SerializeField] private Material _unpoweredIndicatorMaterial;
        
        [SerializeField] private Material _switchedIndicatorMaterial;
        [SerializeField] private Material _unswitchedIndicatorMaterial;

        public bool IsSwitched => _switchNetwork.SignalValue;
        
        protected override void Initialize()
        {
            base.Initialize();
            _switchNetwork.OnSignalChanged += OnSwitchSignalValueChanged;
            SetInitialValueForSwitch(_switchNetwork.SignalValue);
        }

        private void OnSwitchSignalValueChanged(bool value)
        {
            UpdateAppearance();
        }

        private void SetInitialValueForSwitch(bool value)
        {
            UpdateAppearance();
        }

        protected override void SetInitialPoweredState(bool isPowered)
        {
            UpdateAppearance();
        }

        protected override void OnPoweredUp()
        {
            UpdateAppearance();
        }

        protected override void OnPoweredDown()
        {
            UpdateAppearance();
        }

        private void UpdateAppearance()
        {
            SetAppearance(this.IsPowered, this.IsSwitched);
        }
        
        private void SetAppearance(bool isPowered, bool isSwitched)
        {
            var powerMat = isPowered ? _poweredIndicatorMaterial : _unpoweredIndicatorMaterial;
            var switchMat = isSwitched ? _switchedIndicatorMaterial : _unswitchedIndicatorMaterial;
            _powerIndicatorRenderer.material = powerMat;
            _switchIndicatorRenderer.material = switchMat;
        }
        
        public override bool TryBeginInteraction()
        {
            if (!base.TryBeginInteraction()) { return false; }
            
            _switchNetwork.SetAndBroadcastSignal(!_switchNetwork.SignalValue);
            return true;
        }

    }
}