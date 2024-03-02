using UnityEngine;

namespace Project.Interactables
{
    public class FuseBox : PoweredInteractable
    {
        [SerializeField] private MeshRenderer _indicatorRenderer;
        
        [SerializeField] private Material _poweredIndicatorMaterial;
        [SerializeField] private Material _unpoweredIndicatorMaterial;
        
        public override bool TryBeginInteraction()
        {
            _powerNetwork.SetAndBroadcastSignal(!_powerNetwork.SignalValue);
            return true;
        }

        protected override void OnPoweredDown()
        {
            SetAppearance(false);
        }
        
        protected override void OnPoweredUp()
        {
            SetAppearance(true);
        }
        
        protected override void SetInitialPoweredState(bool isPowered)
        {
            SetAppearance(isPowered);
        }
        

        private void SetAppearance(bool isPowered)
        {
            var mat = isPowered ? _poweredIndicatorMaterial : _unpoweredIndicatorMaterial;
            _indicatorRenderer.material = mat;
        }
        
        
    }
}