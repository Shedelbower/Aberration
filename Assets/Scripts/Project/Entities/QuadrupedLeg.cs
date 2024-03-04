using System;
using TMPro;
using UnityEngine;

namespace Project.Entities
{
    public class QuadrupedLeg : MonoBehaviour
    {
        // TODO
        // private static readonly float UPPER_LENGTH = 0.5f;
        // private static readonly float LOWER_LENGTH = 0.5f;
        
        private static readonly float MAX_INCLINE_ANGLE_FOR_FOOT = 60f * Mathf.Deg2Rad;
        private static readonly float FOOT_MIN_BASE_SPEED = 1.0f;
        public static readonly float DESIRED_HEIGHT = 0.5f;
        private static readonly float PREDICTION_TIME_INTERVAL = 0.5f;
        
        private static readonly float TARGET_DISTANCE_THRESHOLD = 0.005f;
        
        private static readonly float MAX_LATERAL_ANGLE = 30f * Mathf.Deg2Rad;
        private static readonly float MAX_WALK_SPEED = 2.0f;
        
        public float ExtensionPercentage => _currExtent / _maxLength;
        
        public Vector3 ShoulderPosition => _upperBone.position;

        private struct FootLocation
        {
            public Vector3 GetPosition(Transform transform)
            {
                if (isLocal)
                {
                    return transform.localToWorldMatrix.MultiplyPoint(this.position);
                }
                return this.position;
            }
            public Vector3 position;
            public bool isGrounded;
            public float initialTravelDistance;
            public bool isLocal;
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
        private FootLocation _desiredFootLocation;
        private FootLocation _targetFootLocation;

        private Vector3 _prevShoulderPosition;
        private Vector3 _shoulderVelocity;
        private float _shoulderSpeed;
        private float _desiredDistanceThreshold;

        private Vector3 _IKFootLiftOffset;

        public enum LegStatus
        {
            None,
            ShouldUpdateTarget,
            WantsToStep,
            HasToStep
        }

        [SerializeField] private LegStatus _status;
        public LegStatus Status { get => _status; private set => _status = value; }

        [SerializeField] private bool _isGrounded;
        public bool IsGrounded
        {
            get => _isGrounded;
            private set => _isGrounded = value;
        }
        
        [SerializeField] private bool _isAtTarget;
        public bool IsAtTarget
        {
            get => _isAtTarget;
            private set => _isAtTarget = value;
        }

        private Rigidbody _rb;

        private bool _initialized = false;
        
        public void Initialize(float fullyExtendedLength, Rigidbody rb)
        {
            _maxLength = fullyExtendedLength;
            _upperLength = _maxLength * 0.5f;
            _lowerLength = _upperLength;

            _rb = rb;

            Reset();

            _initialized = true;
        }

        public void Reset()
        {
            _prevShoulderPosition = this.ShoulderPosition;

            this.FootPosition = this.ShoulderPosition - this.transform.up * DESIRED_HEIGHT;
            _targetFootLocation = new FootLocation()
            {
                position = this.FootPosition,
                isGrounded = true
            };
            
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
            desiredDifference = (_desiredFootLocation.GetPosition(this.transform) - _targetFootLocation.GetPosition(this.transform)).magnitude;
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
                if (this.IsAtTarget && _desiredFootLocation.isGrounded)
                {
                    this.Status = LegStatus.HasToStep;
                }
                
                // Only care if the target position is drastically different that desired position.
                else if (_currExtent > _maxLength)
                {
                    this.Status = LegStatus.HasToStep;
                    // // TODO: Prevent bug where the new desired position is right in front of the current point, causing
                    // // it to stay extended. Maybe a cooldown to legs or something?
                }
                else
                {
                    this.Status = LegStatus.None;   
                }
            }
        }

        private FootLocation CalculateDesiredFootLocation()
        {
            // Predict where the shoulder will be according to its most recent velocity / rotation
            var predicatedShoulderPosition = this.ShoulderPosition;
            
            // predict rotation
            var rotSpeed = _rb.angularVelocity.magnitude;
            var rotAxis = _rb.angularVelocity / rotSpeed;
            var rot = Quaternion.AngleAxis(rotSpeed * PREDICTION_TIME_INTERVAL, rotAxis);
            predicatedShoulderPosition = _rb.position + rot * (predicatedShoulderPosition - _rb.position);
            
            // predict linear movement
            predicatedShoulderPosition += _shoulderVelocity * PREDICTION_TIME_INTERVAL;
            
            
            var downDir = -this.transform.up;
            
            // Cast ray down from predicated position
            RaycastHit hit;

            if (Physics.Raycast(predicatedShoulderPosition, downDir, out hit, _maxLength, _walkableMask, QueryTriggerInteraction.Ignore))
            {

                if (IsValidGroundedFootPosition(hit.normal))
                {
                    return new FootLocation()
                    {
                        position = hit.point,
                        isGrounded = true
                    };   
                }
            }
            
            // Try from current shoulder position
            if (Physics.Raycast(this.ShoulderPosition, downDir, out hit, _maxLength, _walkableMask, QueryTriggerInteraction.Ignore))
            {
                if (IsValidGroundedFootPosition(hit.normal))
                {
                    return new FootLocation()
                    {
                        position = hit.point,
                        isGrounded = true
                    };   
                }
            }
            
            // No valid spot found
            return new FootLocation()
            {
                position = Vector3.down * (0.12f * _maxLength),
                isLocal = true,
                isGrounded = false
            };
        }

        private bool IsValidGroundedFootPosition(Vector3 groundNormal)
        {
            float angleToVertical = Mathf.Acos(Vector3.Dot(groundNormal, Vector3.up));
            return angleToVertical <= MAX_INCLINE_ANGLE_FOR_FOOT;
        }

        public void BeginStep()
        {
            _targetFootLocation = _desiredFootLocation;
            _targetFootLocation.initialTravelDistance = (_targetFootLocation.GetPosition(this.transform) - this.FootPosition).magnitude;
            
            this.IsGrounded = false;
            this.IsAtTarget = false;
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

        private float GetNetFootSpeed(Vector3 targetDir)
        {
            return Vector3.Dot(targetDir, _shoulderVelocity) + GetBaseFootSpeed();
        }

        public void UpdateFootPosition(float deltaTime)
        {
            if (this.IsGrounded) { return; }

            var targetVec = _targetFootLocation.GetPosition(this.transform) - this.FootPosition;
            var targetDist = targetVec.magnitude;
            
            if (targetDist <= TARGET_DISTANCE_THRESHOLD)
            {
                // Reached target
                this.IsAtTarget = true;
                this.IsGrounded = _targetFootLocation.isGrounded;
                this.FootPosition = _targetFootLocation.GetPosition(this.transform); // Snap to target
                return;
            }
            
            // hacky way to pickup foot
            float t = Mathf.Clamp01(targetDist / _targetFootLocation.initialTravelDistance);
            t = Mathf.Sin(t * Mathf.PI);
            float maxLiftHeight = Mathf.Min(0.3f * targetDist, _maxLength * 0.25f);
            _IKFootLiftOffset = (Vector3.up * t * maxLiftHeight);
            
            // Move towards target
            var targetDir = targetVec / targetDist;
            
            // apply shoulder speed to base foot speed
            var footSpeed = GetNetFootSpeed(targetDir);

            // don't overshoot target
            var moveDist = Mathf.Min(footSpeed * deltaTime, targetDist);

            this.FootPosition += targetDir * moveDist;
        }
        

        public void UpdateLegIK()
        {
            var footIKPosition = this.FootPosition + _IKFootLiftOffset;
            
            var shoulderToFootIK = (footIKPosition - this.ShoulderPosition);
            var currExtentIK = shoulderToFootIK.magnitude;
            
            float a = _upperLength;
            float b = _lowerLength;
            float c = currExtentIK;

            if (c <= 0.0f)
            {
                return;
            }
            
            var dir = shoulderToFootIK / c;

            // Law of Cosines
            float value = (a * a + c * c - b * b) / (2 * a * c);
            value = Mathf.Clamp(value, -1f, 1f);
            float theta = Mathf.Acos(value);
            
            var axis = Vector3.Cross(dir, this.transform.forward);
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

            this.ElbowPosition = this.ShoulderPosition + adir * a;

            _lowerBone.position = this.ElbowPosition;
            
            _upperBone.LookAt(_lowerBone.position);
            _lowerBone.LookAt(footIKPosition);

            // Adjust the up vectors of both leg segments
            var evec = (this.ElbowPosition - 0.5f * (this.ShoulderPosition + footIKPosition));

            var upperProjUp = (evec - _upperBone.forward * Vector3.Dot(_upperBone.forward, evec)).normalized;
            var lowerProjUp = (evec - _lowerBone.forward * Vector3.Dot(_lowerBone.forward, evec)).normalized;

            lowerProjUp *= -1f; // Flip lower bone for zig-zag effect

            var angleUpper = Vector3.SignedAngle(_upperBone.up, upperProjUp, _upperBone.forward);
            var angleLower = Vector3.SignedAngle(_lowerBone.up, lowerProjUp, _lowerBone.forward);
            
            _upperBone.rotation = Quaternion.AngleAxis(angleUpper, _upperBone.forward) * _upperBone.rotation;
            _lowerBone.rotation = Quaternion.AngleAxis(angleLower, _lowerBone.forward) * _lowerBone.rotation;
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
            Gizmos.DrawCube(_targetFootLocation.GetPosition(transform), Vector3.one * 0.06f);
            Gizmos.color = _desiredFootLocation.isGrounded ? Color.magenta : Color.cyan;
            Gizmos.DrawCube(_desiredFootLocation.GetPosition(this.transform), Vector3.one * 0.06f);
            Gizmos.DrawWireSphere(_desiredFootLocation.GetPosition(this.transform), _desiredDistanceThreshold);
            
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