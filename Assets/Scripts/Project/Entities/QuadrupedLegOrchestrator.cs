using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UIElements;

namespace Project.Entities
{
    public class QuadrupedLegOrchestrator : MonoBehaviour
    {
        private static readonly float RAGDOLL_TIMER_DURATION = 0.5f; // amount of time with 0 legs grounded before going ragdoll

        public QuadrupedArm Arm => _arm;
        
        [Header("Settings")]
        [SerializeField] private float _linearSpeed = 20f;
        [SerializeField] private float _angularSpeed = 4f;
        
        [SerializeField] private float _fullyExtendedLegLength = 1f;
        [SerializeField] private float _targetHeightPercentageStanding = 0.8f;
        [SerializeField] private float _targetHeightPercentageCrawling = 0.4f;
        [SerializeField] private float _legLiftPower = 40f;
        [SerializeField] private float _gravity = 9.8f;
        [SerializeField] private float _legTiltPower = 1f;
        [SerializeField] private float _legRollPower = 40f;
        
        [Header("Component References")]
        
        [SerializeField] private QuadrupedLeg _legFL;
        [SerializeField] private QuadrupedLeg _legFR;
        [SerializeField] private QuadrupedLeg _legBL;
        [SerializeField] private QuadrupedLeg _legBR;
        [SerializeField] private QuadrupedArm _arm;
        
        [Header("Debug")]
        
        [SerializeField] private float _targetHeightPercentage = 0.8f;
        [SerializeField] private bool _isRagdoll;
        [SerializeField] private bool _isCrawling;

        public float InputLinearMovement { get; set; }
        public float InputAngularMovement { get; set; }

        // TODO: unserialize
        [SerializeField] private float[] _legSmoothGrounded;
        [SerializeField] private float[] _legHeights;
        private int _groundedLegCount;
        private float _ragdollTimer;
        private QuadrupedLeg[] _legs;
        private Rigidbody _rb;
        private PlayableEntity _entity;


        private float _legSpacing;
        private float _defaultDrag;
        
        private Dictionary<QuadrupedLeg, QuadrupedLeg[]> _dependencies;
        
        public void Initialize(PlayableEntity entity, Rigidbody rb)
        {
            _entity = entity;
            _rb = rb;
            
            _legs = new QuadrupedLeg[]
            {
                _legFL,
                _legFR,
                _legBL,
                _legBR
            };
            
            for (int li = 0; li < _legs.Length; li++)
            {
                _legs[li].Initialize(_fullyExtendedLegLength, _rb);
            }

            _legSmoothGrounded = new float[_legs.Length];
            _legHeights = new float[_legs.Length];

            _legSpacing = (_legFL.ShoulderPosition - _legBL.ShoulderPosition).magnitude;
            
            _dependencies = new Dictionary<QuadrupedLeg, QuadrupedLeg[]>();
            _dependencies.Add(_legFL, new []{ _legFR, _legBL});
            _dependencies.Add(_legFR, new []{ _legFL, _legBR});
            _dependencies.Add(_legBL, new []{ _legFL, _legBR});
            _dependencies.Add(_legBR, new []{ _legFR, _legBL});
            
            _arm.Initialize();
        }

        public void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                SetCrawling(!_isCrawling);
            }
        }

        public void OnFixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            UpdateLegStatuses(dt);
            OrchestrateSteps();
            UpdateLegPositions(dt);
            
            _arm.UpdateStatus(dt);
            _arm.UpdatePosition(dt);
            _arm.UpdateIK();

            if (!_isRagdoll)
            {
                ApplyForces(dt);   
            }

            ResetInputs();
        }

        private void SetRagdoll(bool isRagdoll)
        {
            if (_isRagdoll == isRagdoll) { return; }

            _isRagdoll = isRagdoll;
            if (_isRagdoll)
            {
                Debug.Log("[RAGDOLL START]");
                _defaultDrag = _rb.drag;
                _rb.drag = 0.5f;
                _rb.useGravity = true;
            }
            else
            {
                Debug.Log("[RAGDOLL END]");
                _rb.drag = _defaultDrag;
                _rb.useGravity = false;
            }
        }
        
        private void SetCrawling(bool isCrawling)
        {
            // TODO: have speed update to be slower when crawling
            if (_isCrawling == isCrawling) { return; }

            _isCrawling = isCrawling;
            if (_isCrawling)
            {
                Debug.Log("[CRAWLING START]");
                _targetHeightPercentage = _targetHeightPercentageCrawling;
            }
            else
            {
                Debug.Log("[CRAWLING END]");
                _targetHeightPercentage = _targetHeightPercentageStanding;
            }
        }
    

        private void ResetInputs()
        {
            this.InputLinearMovement = 0.0f;
            this.InputAngularMovement = 0.0f;
        }

        private void UpdateLegStatuses(float deltaTime)
        {
            for (int li = 0; li < _legs.Length; li++)
            {
                _legs[li].UpdateStatus(deltaTime);
            }
            
            
            // Update smooth grounded
            float gainSpeed = 10f;
            float lossSpeed = 0.5f;

            _groundedLegCount = 0;
            
            for (int li = 0; li < _legs.Length; li++)
            {
                if (_legs[li].IsGrounded)
                {
                    _groundedLegCount++;
                    _legSmoothGrounded[li] = Mathf.Clamp01(_legSmoothGrounded[li] + gainSpeed * deltaTime);
                }
                else
                {
                    _legSmoothGrounded[li] = Mathf.Clamp01(_legSmoothGrounded[li] - lossSpeed * deltaTime);
                }
            }

            if (_groundedLegCount > 0)
            {
                _ragdollTimer = 0.0f; // Reset ragdoll timer
            }

            if (_isRagdoll && _groundedLegCount >= 2)
            {
                SetRagdoll(false);
            }
            else if (!_isRagdoll && _groundedLegCount == 0)
            {
                _ragdollTimer += deltaTime;
                if (_ragdollTimer >= RAGDOLL_TIMER_DURATION)
                {
                    _ragdollTimer = 0.0f;
                    SetRagdoll(true);
                }
            }

        }

        private void OrchestrateSteps()
        {
            // Handle any legs that have to step
            for (int li = 0; li < _legs.Length; li++)
            {
                if (_legs[li].Status == QuadrupedLeg.LegStatus.HasToStep)
                {
                    _legs[li].BeginStep();
                }
            }
            
            // Handle any legs that only want to step
            for (int li = 0; li < _legs.Length; li++)
            {
                if (_legs[li].Status == QuadrupedLeg.LegStatus.WantsToStep && LegIsSupportedByDependencies(_legs[li]))
                {
                    _legs[li].BeginStep();
                }
            }
        }

        private void UpdateLegPositions(float deltaTime)
        {
            for (int li = 0; li < _legs.Length; li++)
            {
                _legs[li].UpdateFootPosition(deltaTime);
                _legs[li].UpdateLegIK();
            }
        }
        
        private void ApplyForces(float deltaTime)
        {
            RecalculateLegHeights();

            bool FL = _legHeights[0] <= _fullyExtendedLegLength;
            bool FR = _legHeights[1] <= _fullyExtendedLegLength;
            bool BL = _legHeights[2] <= _fullyExtendedLegLength;
            bool BR = _legHeights[3] <= _fullyExtendedLegLength;

            bool canMoveForward = FL || FR;
            bool canMoveBackward = BL || BR;

            float forwardInput = this.InputLinearMovement;
            // if (!canMoveForward) { forwardInput = Mathf.Min(forwardInput, 0.0f); }
            // if (!canMoveBackward) { forwardInput = Mathf.Max(forwardInput, 0.0f); }
            
            // Forces from player input
            Vector3 movementForce = forwardInput * _linearSpeed * this.transform.forward;
            Vector3 angularTorque = this.InputAngularMovement * _angularSpeed * this.transform.up;

            if (_entity.IsActive)
            {
                // Apply movement force
                _rb.AddForce(movementForce, ForceMode.Force);
            
                // Apply torque
                _rb.AddTorque(angularTorque, ForceMode.Force);   
            }

            
            // Height Correction
            float frontsideHeight = Mathf.Min(_legHeights[0], _legHeights[1]);
            float backsideHeight = Mathf.Min(_legHeights[2], _legHeights[3]);

            if (!(FL || FR) && (BL || BR))
            {
                frontsideHeight = 1.5f * backsideHeight;
            }
            
            if (!(BL || BR) && (FL || FR))
            {
                backsideHeight = 1.5f * frontsideHeight;
            }
            
            float centerHeight = (frontsideHeight + backsideHeight) / 2f;
            float targetHeight = _targetHeightPercentage * _fullyExtendedLegLength;

            float heightDelta = targetHeight - centerHeight;

            if (heightDelta > 0.0f) // Add force with legs to lift
            {
                float ht = Mathf.Sign(heightDelta) * Mathf.Clamp01(Mathf.Abs(heightDelta) / targetHeight);
                var heightForce = _legLiftPower * ht * Vector3.up;
                _rb.AddForce(heightForce, ForceMode.Force);
            }
            else // Use gravity to get the rigid body to go down
            {
                float gt = Mathf.Clamp01(Mathf.Abs(heightDelta) / 0.25f); // have gravity slowly falloff to avoid jitter
                _rb.AddForce(gt * _gravity * Vector3.down, ForceMode.Acceleration);
            }
            
            // Tilt Correction
            float frontHeightOffset = ((_legFL.ShoulderPosition.y + _legFR.ShoulderPosition.y) - (_legBL.ShoulderPosition.y + _legBR.ShoulderPosition.y))/2.0f;
            
            float opp = frontHeightOffset;
            float hypo = _legSpacing;
            float ratio = Mathf.Clamp(opp / hypo, -1f, 1f);
            var currTilt = Mathf.Asin(ratio);
            
            
            opp = (frontsideHeight + frontHeightOffset) - backsideHeight;
            hypo = _legSpacing;
            ratio = Mathf.Clamp(opp / hypo, -1f, 1f);
            var targetTilt = Mathf.Asin(ratio);

            var tiltDelta = targetTilt - currTilt;
            var tt = tiltDelta / Mathf.PI;
            var tiltTorque = _legTiltPower * tt * this.transform.right;
            _rb.AddTorque(tiltTorque, ForceMode.Force);
            
            // Roll correction
            var currRoll = Mathf.Acos(Vector3.Dot(this.transform.right, Vector3.up)) - Mathf.PI * 0.5f;
            var sign = Vector3.Dot(Vector3.Cross(this.transform.right, Vector3.up), this.transform.forward);
            currRoll *= -sign;
            var targetRoll = 0.0f;

            var rollDelta = targetRoll - currRoll;
            var rollTorque = _legRollPower * this.transform.forward * rollDelta;
            _rb.AddTorque(rollTorque, ForceMode.Force);
        }

        private void RecalculateLegHeights()
        {
            float maxOrDefaultHeight = _fullyExtendedLegLength * 10.0f;
            for (int li = 0; li < _legs.Length; li++)
            {
                var legPos = _legs[li].ShoulderPosition;
                RaycastHit hit;
                var walkableMask = LayerMask.GetMask("Walkable");
                
                _legHeights[li] = maxOrDefaultHeight;
                
                if (Physics.Raycast(legPos, Vector3.down, out hit, maxOrDefaultHeight, walkableMask, QueryTriggerInteraction.Ignore))
                {
                    _legHeights[li] = hit.distance;
                    
                }
            }
        }

        private bool LegIsSupportedByDependencies(QuadrupedLeg leg)
        {
            var supports = _dependencies[leg];
            for (int i = 0; i < supports.Length; i++)
            {
                if (!supports[i].IsGrounded)
                {
                    return false;
                }
            }

            return true;
        }
    }
}