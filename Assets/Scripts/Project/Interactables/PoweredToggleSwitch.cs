using UnityEngine;

namespace Project.Interactables
{
    public class PoweredToggleSwitch : PoweredInteractable
    {
        [SerializeField] private SignalNetwork<bool> _switchNetwork;
        [SerializeField] private MeshRenderer[] _powerIndicatorRenderers;
        [SerializeField] private MeshRenderer[] _switchIndicatorRenderers;

        private Material _poweredIndicatorMaterial;
        private Material _unpoweredIndicatorMaterial;
        
        private Material _switchedIndicatorMaterial;
        private Material _unswitchedIndicatorMaterial;

        public bool IsSwitched => _switchNetwork.SignalValue;
        
        protected override void Initialize()
        {
            base.Initialize();
            _switchNetwork.OnSignalChanged += OnSwitchSignalValueChanged;
            SetInitialValueForSwitch(_switchNetwork.SignalValue);

            LoadMaterials();
        }

        private void LoadMaterials()
        {
            if (_poweredIndicatorMaterial != null) { return; }
            
            _poweredIndicatorMaterial = Resources.Load<Material>("Materials/Indicators/Power On");
            _unpoweredIndicatorMaterial = Resources.Load<Material>("Materials/Indicators/Power Off");
            
            _switchedIndicatorMaterial = Resources.Load<Material>("Materials/Indicators/Open");
            _unswitchedIndicatorMaterial = Resources.Load<Material>("Materials/Indicators/Closed");
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
            LoadMaterials();
            
            var powerMat = isPowered ? _poweredIndicatorMaterial : _unpoweredIndicatorMaterial;
            var switchMat = isSwitched ? _switchedIndicatorMaterial : _unswitchedIndicatorMaterial;
            
            for (int mi = 0; mi < _powerIndicatorRenderers.Length; mi++)
            {
                _powerIndicatorRenderers[mi].material = powerMat;
            }
            
            for (int mi = 0; mi < _switchIndicatorRenderers.Length; mi++)
            {
                _switchIndicatorRenderers[mi].material = switchMat;
            }
        }
        
        public override bool TryBeginInteraction()
        {
            if (!base.TryBeginInteraction()) { return false; }
            
            _switchNetwork.SetAndBroadcastSignal(!_switchNetwork.SignalValue);
            return true;
        }

    }
}