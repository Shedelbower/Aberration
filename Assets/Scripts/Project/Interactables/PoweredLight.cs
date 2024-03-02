using UnityEngine;

namespace Project.Interactables
{
    public class PoweredLight : PoweredComponent
    {
        [SerializeField] private Light _light;
        
        protected override void SetInitialPoweredState(bool isPowered)
        {
            _light.enabled = isPowered;
        }

        protected override void OnPoweredUp()
        {
            _light.enabled = true;
        }
        
        protected override void OnPoweredDown()
        {
            _light.enabled = false;
        }
    }
}