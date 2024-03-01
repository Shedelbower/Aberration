using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Project.Entities
{
    public class RoverLeg : MonoBehaviour
    {
        private static readonly float GRAVITY = 9.8f;
        
        public bool IsGrounded { get; private set; }

        [SerializeField] private Transform _segmentA;
        [SerializeField] private Transform _segmentB;
        
        [SerializeField] private RoverLeg[] _dependentLegs;
        
        private Vector3 ShoulderPosition => this.transform.position;
        
        private Vector3 _elbowPosition;
        private Vector3 _footPosition;
        private Vector3 _targetFootPosition;
        private float _maxLength = 1.0f;
        private float _targetLength = 0.9f;

        private Vector3 _tempDesiredFootPos;

        private float _segmentALength;
        private float _segmentBLength;

        private bool _initialized;

        public void Initialize()
        {
            _initialized = true;
            
            // Set leg segment lengths based on a percentage of the total max leg length
            float aPercentage = 0.5f;
            _segmentALength = _maxLength * aPercentage;
            _segmentBLength = _maxLength - _segmentALength;
            
            var scaleA = _segmentA.localScale;
            scaleA.z = _segmentALength;
            _segmentA.localScale = scaleA;
            var scaleB = _segmentB.localScale;
            scaleB.z = _segmentBLength;
            _segmentB.localScale = scaleB;
            
            // Set initial positions
            _footPosition = this.transform.position + -this.transform.up;
            _targetFootPosition = _footPosition;

            UpdateLegPosition(Vector3.zero);
        }

        private bool CanStep()
        {
            // Only allowed to step if all dependent legs are grounded
            for (int li = 0; li < _dependentLegs.Length; li++)
            {
                if (!_dependentLegs[li].IsGrounded)
                {
                    return false;
                }
            }

            return true;
        }

        private Vector3 GetDesiredFootPosition(Vector3 baseVelocity)
        {
            var downDir = -this.transform.up;
            var rotAxis = this.transform.right;
            var at = Mathf.InverseLerp(0.0f, 3.0f, baseVelocity.magnitude);
            var angle = Mathf.Lerp(0.0f, 45f * Mathf.Deg2Rad, at);

            var rot = Quaternion.AngleAxis(-angle * Mathf.Rad2Deg, rotAxis);
            var rayDir = rot * downDir;
            var ray = new Ray(this.ShoulderPosition, rayDir);

            if (TryCastFootPosition(ray, out RaycastHit hit))
            {
                _tempDesiredFootPos = hit.point;
            }
            
            return _tempDesiredFootPos;
        }

        private bool TryCastFootPosition(Ray ray, out RaycastHit hit)
        {
            var mask = LayerMask.GetMask("Walkable"); // TODO: Cache at start
            return Physics.Raycast(ray, out hit, _maxLength, mask, QueryTriggerInteraction.Ignore);
        }

        public void UpdateLegPosition(Vector3 velocity)
        {
            // Check if we need to take other step
            if (this.CanStep())
            {
                var speed = velocity.magnitude;
                var desiredPosition = GetDesiredFootPosition(velocity);
                var offsetVec = desiredPosition - _targetFootPosition;
                var offset = offsetVec.magnitude;
                var st = Mathf.InverseLerp(0.0f, 2.0f, speed);

                var stepThresholdDistance = Mathf.Lerp(0.2f, 1.0f, st);
                if (offset > stepThresholdDistance && this.IsGrounded)
                {
                    // Step!
                    _targetFootPosition = desiredPosition;
                    this.IsGrounded = false;
                }    
            }
            
            // Check if foot has reached its target to be grounded.
            if (!this.IsGrounded)
            {
                // Move foot towards target
                _footPosition = Vector3.Lerp(_targetFootPosition, _footPosition, 0.6f);

                if ((_footPosition - _targetFootPosition).magnitude < 0.01f)
                {
                    IsGrounded = true;
                }
            }
            
            var startPos = this.ShoulderPosition;
            var endPos = _footPosition;
            
            var vec = (endPos - startPos);

            float a = _segmentALength;
            float b = _segmentBLength;
            float c = vec.magnitude;
            
            var dir = vec / c;

            // Law of Cosines
            float value = (a * a + c * c - b * b) / (2 * a * c);
            value = Mathf.Clamp(value, -1f, 1f);
            float theta = Mathf.Acos(value);
            
            var axis = this.transform.right;
            var adir = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, axis) * dir;

            var midPos = startPos + adir * a;

            _segmentA.position = startPos;
            _segmentB.position = midPos;

            _elbowPosition = midPos;
            
            _segmentA.LookAt(midPos);
            _segmentB.LookAt(endPos);
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
            
            Vector3 force = Vector3.zero;
            
            if (distance <= _targetLength)
            {
                var delta = _targetLength - distance;
                var magnitude = delta;
                force = GRAVITY * tr.up * magnitude;
            }
            rb.AddForceAtPosition(force, this.transform.position, ForceMode.Force);
            
        }


        private void OnDrawGizmos()
        {
            if (!_initialized) { return; }
            
            Gizmos.color = this.IsGrounded ? Color.gray : Color.red;
            Gizmos.DrawCube(this.ShoulderPosition, Vector3.one * 0.1f);
            Gizmos.color = this.IsGrounded ? Color.gray : Color.green;
            Gizmos.DrawCube(_elbowPosition, Vector3.one * 0.1f);
            Gizmos.color = this.IsGrounded ? Color.gray : Color.blue;
            Gizmos.DrawCube(_footPosition, Vector3.one * 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(_targetFootPosition, Vector3.one * 0.06f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(_tempDesiredFootPos, Vector3.one * 0.06f);
            

            Gizmos.color = Color.white;
            Gizmos.DrawLine(this.ShoulderPosition, _elbowPosition);
            Gizmos.DrawLine(_elbowPosition, _footPosition);
        }
    }
}