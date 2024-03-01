using Unity.Mathematics;
using UnityEngine;

namespace Project.Entities
{
    public class RoverLeg : MonoBehaviour
    {
        private static readonly float GRAVITY = 9.8f;
        
        private Vector3 _footPosition;
        private float _maxLength = 2.0f;
        private float _targetLength = 1.0f;

        public void Initialize()
        {
            
        }

        public void ApplyForceToRover(Rigidbody rb)
        {
            var tr = this.transform;
            var ray = new Ray(tr.position, -tr.up);
            var mask = LayerMask.GetMask("Walkable");

            float distance = float.MaxValue;

            if (Physics.Raycast(ray, out RaycastHit hit, _maxLength, mask, QueryTriggerInteraction.Ignore))
            {
                distance = hit.distance;
            }
            
            Vector3 force;
            
            if (distance <= _targetLength)
            {
                var delta = _targetLength - distance;
                var magnitude = delta;
                force = GRAVITY * tr.up * magnitude;
            }
            else
            {
                float t = Mathf.Clamp01((distance - _targetLength) / _maxLength);
                t = t * t;
                force = GRAVITY * Vector3.down * t;
            }
            
            rb.AddForceAtPosition(force, this.transform.position, ForceMode.Force);
        }
        
        
    }
}