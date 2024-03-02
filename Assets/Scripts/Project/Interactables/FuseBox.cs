using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace Project.Interactables
{
    public class FuseBox : Interactable, IPowered
    {
        [SerializeField] private PowerGrid _grid;
        [SerializeField] private MeshRenderer _indicatorRenderer;
        
        [SerializeField] private Material _poweredIndicatorMaterial;
        [SerializeField] private Material _unpoweredIndicatorMaterial;
        
        public override bool TryBeginInteraction()
        {
            //if (!base.TryBeginInteraction()) { return false; }
            
            _grid.TogglePowered();
            return true;
        }

        public void OnPoweredDown()
        {
            SetAppearance(false);
        }
        
        public void OnPoweredUp()
        {
            SetAppearance(true);
        }
        
        public void SetInitialPoweredState(bool isPowered)
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