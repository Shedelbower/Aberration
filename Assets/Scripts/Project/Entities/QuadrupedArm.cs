using UnityEngine;

namespace Project.Entities
{
    public class QuadrupedArm : MonoBehaviour
    {
        [SerializeField] private float _upperLength = 0.5f;
        [SerializeField] private float _lowerLength = 0.5f;
        [SerializeField] private float _clawLength = 0.15f;
        
        [SerializeField] private Transform _upperBone;
        [SerializeField] private Transform _lowerBone;
        [SerializeField] private Transform _clawBone;
        [SerializeField] private Transform _touchTarget;

        private Vector3 ShoulderPosition => _upperBone.position;

        private Vector3 ElbowPosition
        {
            get => _lowerBone.position;
            set => _lowerBone.position = value;
        }
        private Vector3 ClawPosition
        {
            get => _clawBone.position;
            set => _clawBone.position = value;
        }

        private Vector3 _shoulderVelocity;
        private Vector3 _prevShoulderPosition;

        private Vector3 TouchPosition { get; set; }

        private bool _initialized;
        public void Initialize()
        {
            _initialized = true;
            // _touchTarget = new Vector3(0, 0.1f, 0.2f);
        }

        public void UpdateStatus(float deltaTime)
        {
            _shoulderVelocity = (this.ShoulderPosition - _prevShoulderPosition) / deltaTime;
            _prevShoulderPosition = this.ShoulderPosition;
        }

        public void UpdatePosition(float deltaTime)
        {
            // Move touch position towards touch target
            var toTarget = _touchTarget.position - this.TouchPosition;
            var targetDir = toTarget.normalized;

            float speed = GetNetSpeed(targetDir);

            float moveDist = Mathf.Min(speed * deltaTime, toTarget.magnitude);
            this.TouchPosition += moveDist * targetDir;
        }
        
        public void UpdateIK()
        {
            var shoulderToTarget = (_touchTarget.position - this.ShoulderPosition);
            shoulderToTarget.y = 0.0f;
            var targetDirHorizontal = shoulderToTarget.normalized;
            var wristPosition = this.TouchPosition + (-targetDirHorizontal) * _clawLength;
            
            var shoulderToWristIK = (wristPosition - this.ShoulderPosition);
            var currExtentIK = shoulderToWristIK.magnitude;
            
            float a = _upperLength;
            float b = _lowerLength;
            float c = currExtentIK;

            if (c <= 0.0f)
            {
                return;
            }
            
            var dir = shoulderToWristIK / c;

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

            if (Vector3.Dot(axis, this.transform.right) > 0)
            {
                axis *= -1f;
            }
            var adir = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, axis) * dir;

            Debug.DrawLine(this.transform.position, this.transform.position + adir);
            
            this.ElbowPosition = this.ShoulderPosition + adir * a;
            this.ClawPosition = wristPosition;

            _lowerBone.position = this.ElbowPosition;
            
            _upperBone.LookAt(this.ElbowPosition);
            _lowerBone.LookAt(this.ClawPosition);
            _clawBone.LookAt(this.TouchPosition);

            // Adjust the up vectors of both leg segments
            var evec = (this.ElbowPosition - 0.5f * (this.ShoulderPosition + wristPosition));
            
            var upperProjUp = (evec - _upperBone.forward * Vector3.Dot(_upperBone.forward, evec)).normalized;
            var lowerProjUp = (evec - _lowerBone.forward * Vector3.Dot(_lowerBone.forward, evec)).normalized;
            
            lowerProjUp *= -1f; // Flip lower bone for zig-zag effect
            
            var angleUpper = Vector3.SignedAngle(_upperBone.up, upperProjUp, _upperBone.forward);
            var angleLower = Vector3.SignedAngle(_lowerBone.up, lowerProjUp, _lowerBone.forward);
            
            _upperBone.rotation = Quaternion.AngleAxis(angleUpper, _upperBone.forward) * _upperBone.rotation;
            _lowerBone.rotation = Quaternion.AngleAxis(angleLower, _lowerBone.forward) * _lowerBone.rotation;
            
            
        }

        private float GetBaseSpeed()
        {
            return 1f;
        }
        
        private float GetNetSpeed(Vector3 targetDir)
        {
            return Vector3.Dot(targetDir, _shoulderVelocity) + GetBaseSpeed();
        }
        
        private void OnDrawGizmos()
        {
            if (!_initialized) { return; }
            
            // Positions
            Gizmos.color = Color.red;
            Gizmos.DrawCube(this.ShoulderPosition, Vector3.one * 0.05f);
            Gizmos.color = Color.green;
            Gizmos.DrawCube(this.ElbowPosition, Vector3.one * 0.05f);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(this.ClawPosition, Vector3.one * 0.05f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(this.TouchPosition, Vector3.one * 0.05f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(_touchTarget.position, Vector3.one * 0.06f);
            
            // Legs
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(this.ShoulderPosition, this.ElbowPosition);
            Gizmos.DrawLine(this.ElbowPosition, this.ClawPosition);
            Gizmos.DrawLine(this.ClawPosition, this.TouchPosition);
            
            // Velocities
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.ShoulderPosition, this.ShoulderPosition + _shoulderVelocity);
        }
    }
}