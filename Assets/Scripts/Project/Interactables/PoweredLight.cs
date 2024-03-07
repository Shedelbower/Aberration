using Unity.VisualScripting;
using UnityEngine;

namespace Project.Interactables
{
    public class PoweredLight : PoweredComponent
    {
        [SerializeField] private bool _invertedPowerSignal;
        [SerializeField] private Light[] _lights;
        
        protected override void SetInitialPoweredState(bool isPowered)
        {
            for (int li = 0; li < _lights.Length; li++)
            {
                _lights[li].enabled = _invertedPowerSignal ? !isPowered : isPowered;
            }
        }

        protected override void OnPoweredUp()
        {
            for (int li = 0; li < _lights.Length; li++)
            {
                _lights[li].enabled = _invertedPowerSignal ? false : true;
            }
        }
        
        protected override void OnPoweredDown()
        {
            for (int li = 0; li < _lights.Length; li++)
            {
                _lights[li].enabled = _invertedPowerSignal ? true : false;
            }
        }
    }
}