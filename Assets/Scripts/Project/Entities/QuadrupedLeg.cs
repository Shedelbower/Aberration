using System;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace Project.Entities
{
    public class QuadrupedLeg : MonoBehaviour
    {
        // TODO
        private static readonly float UPPER_LENGTH = 0.5f;
        private static readonly float LOWER_LENGTH = 0.5f;
        
        private static readonly float FOOT_MIN_BASE_SPEED = 0.5f;
        public static readonly float DESIRED_HEIGHT = 0.5f;
        private static readonly float PREDICTION_TIME_INTERVAL = 0.7f;
        
        private static readonly float TARGET_DISTANCE_THRESHOLD = 0.005f;
        
        private static readonly float MAX_LATERAL_ANGLE = 20f * Mathf.Deg2Rad;
        private static readonly float MAX_WALK_SPEED = 2.0f;
        
        public float ExtensionPercentage => _currExtent / _maxLength;
        
        public Vector3 ShoulderPosition => _upperBone.position;

        private struct DesiredFootLocation
        {
            public Vector3 position;
            public bool isGrounded;
        }

        private Vector3 ElbowPosition
        {
            get => _lowerBone.position;
            set => _lowerBone.position = value;
        }
        private Vector3 FootPosition { get; set; }
        
        [SerializeField] private Transform _upperBone;
        [SerializeField] private Transform _lowerBone;
        [SerializeField] private LayerMask _walkableMask;

        private float _upperLength;
        private float _lowerLength;
        private float _maxLength;

        private Vector3 _shoulderToFoot;
        private float _currExtent;
        private float _lateralAngle;
        private DesiredFootLocation _desiredFootLocation;
        private Vector3 _targetFootPosition;

        private Vector3 _prevShoulderPosition;
        private Vector3 _shoulderVelocity;
        private float _shoulderSpeed;
        private float _desiredDistanceThreshold;

        public enum LegStatus
        {
            None,
            ShouldUpdateTarget,
            WantsToStep,
            HasToStep
        }

        [SerializeField] private LegStatus _status;
        public LegStatus Status { get => _status; private set => _status = value; }
        
        public bool IsGrounded { get; private set; }
        public bool ReachedDesiredLocation { get; private set; }

        private bool _initialized = false;
        
        public void Initialize()
        {
            _upperLength = UPPER_LENGTH;
            _lowerLength = LOWER_LENGTH;
            _maxLength = _upperLength + _lowerLength;

            Reset();

            _initialized = true;
        }

        public void Reset()
        {
            _prevShoulderPosition = this.ShoulderPosition;

            this.FootPosition = this.ShoulderPosition - this.transform.up * DESIRED_HEIGHT;
            _targetFootPosition = this.FootPosition;
            
            this.ElbowPosition = (this.FootPosition + this.ShoulderPosition) / 2;
            
            this.Status = LegStatus.None;
            this.IsGrounded = false;
        }

        private float CalculateDesiredDistanceThreshold()
        {
            float st = Mathf.InverseLerp(0.0f, MAX_WALK_SPEED, _shoulderSpeed);
            float threshold = Mathf.Lerp(0.05f, 1.5f, st);
            return threshold;
        }

        public void UpdateStatus(float deltaTime)
        {
            // Calculate up-to-date information
            
            _shoulderVelocity = (this.ShoulderPosition - _prevShoulderPosition) / deltaTime;
            _prevShoulderPosition = this.ShoulderPosition;
            _shoulderSpeed = _shoulderVelocity.magnitude;
            
            _shoulderToFoot = (this.FootPosition - this.ShoulderPosition);

            _currExtent = _shoulderToFoot.magnitude;

            // var footDir = _shoulderToFoot / _currExtent;
            
            _lateralAngle = CalculateLateralAngle();

            _desiredFootLocation = CalculateDesiredFootLocation();

            float desiredDifference = float.MinValue;
            desiredDifference = (_desiredFootLocation.position - _targetFootPosition).magnitude;
            _desiredDistanceThreshold = CalculateDesiredDistanceThreshold();

            
            // Determine status

            if (this.IsGrounded)
            {
                if (_currExtent > _maxLength || _lateralAngle > MAX_LATERAL_ANGLE)
                {
                    this.Status = LegStatus.HasToStep;
                }
                else if (_desiredFootLocation.isGrounded && desiredDifference > _desiredDistanceThreshold)
                {
                    this.Status = LegStatus.WantsToStep;
                }
                else
                {
                    this.Status = LegStatus.None;   
                }
            }
            else
            {
                // Only care if the target position is drastically different that desired position.
                
                this.Status = LegStatus.None;
            }
        }

        // private float EstimateStepTime(float stepDistance)
        // {
        //     return stepDistance / FOOT_BASE_SPEED;
        // }

        private DesiredFootLocation CalculateDesiredFootLocation()
        {
            // Predict where the shoulder will be according to its most recent velocity
            var predicatedShoulderPosition = this.ShoulderPosition + _shoulderVelocity * PREDICTION_TIME_INTERVAL;
            var downDir = -this.transform.up;
            
            // Cast ray down from predicated position
            RaycastHit hit;

            if (Physics.Raycast(predicatedShoulderPosition, downDir, out hit, _maxLength, _walkableMask, QueryTriggerInteraction.Ignore))
            {
                return new DesiredFootLocation()
                {
                    position = hit.point,
                    isGrounded = true
                };
            }
            
            // Try from current shoulder position
            if (Physics.Raycast(this.ShoulderPosition, downDir, out hit, _maxLength, _walkableMask, QueryTriggerInteraction.Ignore))
            {
                return new DesiredFootLocation()
                {
                    position = hit.point,
                    isGrounded = true
                };
            }
            
            // No valid spot found
            return new DesiredFootLocation()
            {
                position = this.ShoulderPosition + downDir * 0.1f,
                isGrounded = false
            };
        }

        public void BeginStep()
        {
            _targetFootPosition = _desiredFootLocation.position;
            this.IsGrounded = false;
            this.Status = LegStatus.None;
        }
        


        /// <summary>
        /// Calculate the lateral angle between the down direction and the direction from the should to the foot.
        /// Positive angle means pointed outward, negative means inward. All arguments should be normalized.
        /// </summary>
        private float CalculateLateralAngle()
        {
            // TODO: Need only lateral component of footDir
            //this.transform.forward, -this.transform.up, footDir
            //Vector3 forward, Vector3 down, Vector3 footDir

            var down = -this.transform.up;
            var forward = this.transform.forward;
            var lateralFoot = _shoulderToFoot - forward * Vector3.Dot(forward, _shoulderToFoot);

            lateralFoot = lateralFoot.normalized;
            
            if (lateralFoot.sqrMagnitude < 0.001f)
            {
                return 0.0f;
            }
            
            var angleRad = Mathf.Acos(Vector3.Dot(down, lateralFoot));
            return angleRad;
            // return SignedAngle(down, lateralFoot, forward);
            // return Mathf.Abs(SignedAngle(down, lateralFoot, forward));
        }

        private float SignedAngle(Vector3 dir0, Vector3 dir1, Vector3 axis)
        {
            var angleRad = Mathf.Acos(Vector3.Dot(dir0, dir1));
            var latAxis = Vector3.Cross(dir0, dir1);
            if (Vector3.Dot(axis, latAxis) < 0.0f)
            {
                angleRad *= -1;
            }
            return angleRad;
        }

        private float GetBaseFootSpeed()
        {
            return Mathf.Max(FOOT_MIN_BASE_SPEED, _shoulderSpeed * 2f);
        }

        public void UpdateFootPosition(float deltaTime)
        {
            if (this.IsGrounded) { return; }

            var targetVec = _targetFootPosition - this.FootPosition;
            var targetDist = targetVec.magnitude;
            
            if (targetDist <= TARGET_DISTANCE_THRESHOLD)
            {
                // Reached target
                this.IsGrounded = true;
                return;
            }
            
            // Move towards target
            var targetDir = targetVec / targetDist;

            // apply shoulder speed to base foot speed
            var footSpeed = Vector3.Dot(targetDir, _shoulderVelocity) + GetBaseFootSpeed();

            // don't overshoot target
            var moveDist = Mathf.Min(footSpeed * deltaTime, targetDist);

            this.FootPosition += targetDir * moveDist;
        }
        

        public void UpdateLegIK()
        {
            float a = UPPER_LENGTH;
            float b = LOWER_LENGTH;
            float c = _currExtent;

            if (c <= 0.0f)
            {
                return;
            }
            
            var dir = _shoulderToFoot / c;

            // Law of Cosines
            float value = (a * a + c * c - b * b) / (2 * a * c);
            value = Mathf.Clamp(value, -1f, 1f);
            float theta = Mathf.Acos(value);
            
            var axis = Vector3.Cross(_shoulderToFoot / _currExtent, this.transform.forward);
            axis = axis.normalized; // TODO: Is this needed?
            if (axis.sqrMagnitude == 0)
            {
                axis = this.transform.right;
            }

            if (Vector3.Dot(axis, this.transform.right) < 0)
            {
                axis *= -1f;
            }
            var adir = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, axis) * dir;

            // var midPos = this.ShoulderPosition + adir * a;
            this.ElbowPosition = this.ShoulderPosition + adir * a;
            
            
            // _upperBone.position = this.ShoulderPosition;
            _lowerBone.position = this.ElbowPosition;
            
            _upperBone.LookAt(_lowerBone.position);
            // TODO: Fix rotation so lower and upper bone stay on same plane
            _lowerBone.LookAt(this.FootPosition);
        }
        
        private void OnDrawGizmos()
        {
            if (!_initialized) { return; }
            
            // Positions
            Gizmos.color = this.IsGrounded ? new Color(77f/255, 36f/255, 26f/255) : Color.red;
            Gizmos.DrawCube(this.ShoulderPosition, Vector3.one * 0.1f);
            Gizmos.color = this.IsGrounded ? new Color(26f/255, 77f/255, 26f/255) : Color.green;
            Gizmos.DrawCube(this.ElbowPosition, Vector3.one * 0.1f);
            Gizmos.color = this.IsGrounded ? new Color(20f/255, 36f/255, 77f/255) : Color.blue;
            Gizmos.DrawCube(this.FootPosition, Vector3.one * 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(_targetFootPosition, Vector3.one * 0.06f);
            Gizmos.color = _desiredFootLocation.isGrounded ? Color.magenta : Color.cyan;
            Gizmos.DrawCube(_desiredFootLocation.position, Vector3.one * 0.06f);
            Gizmos.DrawWireSphere(_desiredFootLocation.position, _desiredDistanceThreshold);
            
            // Legs
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(this.ShoulderPosition, this.ElbowPosition);
            Gizmos.DrawLine(this.ElbowPosition, this.FootPosition);
            
            // Velocities
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.ShoulderPosition, this.ShoulderPosition + _shoulderVelocity);
        }
        
        
    }
}