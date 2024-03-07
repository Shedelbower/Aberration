using Unity.VisualScripting;
using UnityEngine;

namespace Project.Interactables
{
    public class EmergencyLight : PoweredLight
    {
        [SerializeField] private Transform _rotationBase;
        [SerializeField] private float _rotationSpeed = 60f;


        private void Update()
        {
            if (!this.IsPowered)
            {
                var angle = _rotationSpeed * Time.deltaTime;
                var rot = Quaternion.AngleAxis(angle, this.transform.up);
                _rotationBase.rotation = rot * _rotationBase.rotation;
            }
        }
    }
}