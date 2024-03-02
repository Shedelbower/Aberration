using UnityEngine;

namespace Project.Interactables
{
    public class PoweredLight : MonoBehaviour, IPowered
    {
        [SerializeField] private Light _light;

        private void Initialize()
        {
            
        }

        public void SetInitialPoweredState(bool isPowered)
        {
            _light.enabled = isPowered;
        }

        public void OnPoweredUp()
        {
            _light.enabled = true;
        }
        
        public void OnPoweredDown()
        {
            _light.enabled = false;
        }
    }
}