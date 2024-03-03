using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace Project.Entities
{
    public class QuadrupedLegOrchestrator : MonoBehaviour
    {
        [SerializeField] private QuadrupedLeg _legFL;
        [SerializeField] private QuadrupedLeg _legFR;
        [SerializeField] private QuadrupedLeg _legBL;
        [SerializeField] private QuadrupedLeg _legBR;
        
        private QuadrupedLeg[] _legs;

        private Dictionary<QuadrupedLeg, QuadrupedLeg[]> _dependencies;
        
        public void Initialize()
        {
            _legs = new QuadrupedLeg[]
            {
                _legFL,
                _legFR,
                _legBL,
                _legBR
            };
            
            for (int li = 0; li < _legs.Length; li++)
            {
                _legs[li].Initialize();
            }


            _dependencies = new Dictionary<QuadrupedLeg, QuadrupedLeg[]>();
            _dependencies.Add(_legFL, new []{ _legFR, _legBL});
            _dependencies.Add(_legFR, new []{ _legFL, _legBR});
            _dependencies.Add(_legBL, new []{ _legFL, _legBR});
            _dependencies.Add(_legBR, new []{ _legFR, _legBL});
        }

        public void OnFixedUpdate()
        {
            UpdateLegStatuses(Time.fixedDeltaTime);
        }

        private void UpdateLegStatuses(float deltaTime)
        {
            for (int li = 0; li < _legs.Length; li++)
            {
                _legs[li].UpdateStatus(deltaTime);
            }
            
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
            
            for (int li = 0; li < _legs.Length; li++)
            {
                _legs[li].UpdateFootPosition(deltaTime);
                _legs[li].UpdateLegIK();
            }
        }
        
        private void DetermineWhoSteps(float deltaTime)
        {
            
        }

        public void ApplyForces(Rigidbody rb, Vector3 movementForce, Vector3 turnTorque, float powerScale)
        {
            int groundedCount = 0;
            for (int li = 0; li < _legs.Length; li++)
            {
                if (_legs[li].IsGrounded)
                {
                    groundedCount++;
                }
            }

            float moveScale = 1.0f;
            float turnScale = 1.0f;
            
            if (groundedCount > 1)
            {
                rb.drag = 5f;
            }
            else if (groundedCount == 1)
            {
                rb.drag = 2.5f;
                moveScale = 0.5f;
                turnScale = 0.5f;
            }
            else
            {
                rb.drag = 1f;
                moveScale = 0.0f;
                turnScale = 0.0f;
            }
            
            if (groundedCount <= 0) // Skip applying forces
            {
                return; // No forces
            }
            
            // Apply movement force
            rb.AddForce(movementForce * moveScale, ForceMode.Force);
            
            // Apply torque
            rb.AddTorque(turnTorque * turnScale, ForceMode.Force);
            
            // Apply vertical forces
            for (int li = 0; li < _legs.Length; li++)
            {
                var leg = _legs[li];

                float escale = leg.ExtensionPercentage;
                escale = (1.0f - escale);
                escale = Mathf.Lerp(0.9f, 1.0f, escale);
                
                float forceScale = powerScale * escale;
                if (!leg.IsGrounded)
                {
                    forceScale *= 0.97f;
                }
                Debug.DrawLine(leg.ShoulderPosition, leg.ShoulderPosition + Vector3.up * forceScale * 0.1f);
                rb.AddForceAtPosition(Vector3.up * forceScale, leg.ShoulderPosition, ForceMode.Force);
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