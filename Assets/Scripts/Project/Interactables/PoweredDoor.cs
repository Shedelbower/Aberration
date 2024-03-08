using System;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Project.Interactables
{
    public class PoweredDoor : PoweredComponent
    {
        public bool IsOpen => _signalNetwork.SignalValue;
        
        [Header("Settings")]
        [SerializeField] private float _sideOpenDistance = 0.9f;
        [SerializeField] private float _rotateDuration = 0.3f;
        [SerializeField] private float _slideDuration = 0.5f;
        
        [SerializeField] private AnimationCurve _rotateAnimationCurve = AnimationCurve.EaseInOut(0,0,1,1);
        [SerializeField] private AnimationCurve _slideAnimationCurve = AnimationCurve.EaseInOut(0,0,1,1);
        
        [Header("Component References")]
        [SerializeField] private SignalNetwork<bool> _signalNetwork;
        
        [SerializeField] private Transform[] _obstructionDetectionZones;
        
        [SerializeField] private Transform[] _rotators;
        [SerializeField] private Transform[] _sides;
        
        [SerializeField] private MeshRenderer[] _powerIndicatorRenderers;
        [SerializeField] private MeshRenderer[] _openIndicatorRenderers;

        [Header("Debug")]
        [SerializeField] private float _animationOpenPercentage;
        
        private Material _powerOnMaterial;
        private Material _powerOffMaterial;
        
        private Material _openMaterial;
        private Material _closedMaterial;

        private bool _shouldAnimate;

        private bool _animationIsOpening;

        protected override void Initialize()
        {
            base.Initialize();
            
            _powerOnMaterial = Resources.Load<Material>("Materials/Indicators/Power On");
            _powerOffMaterial = Resources.Load<Material>("Materials/Indicators/Power Off");
            
            _openMaterial = Resources.Load<Material>("Materials/Indicators/Open");
            _closedMaterial = Resources.Load<Material>("Materials/Indicators/Closed");

            _signalNetwork.OnSignalChanged += OnOpenSignalChanged;
        }
        
        protected override void SetInitialPoweredState(bool isPowered)
        {
            UpdatePowerIndicators();
        }

        protected override void OnPoweredUp()
        {
            UpdatePowerIndicators();
        }

        protected override void OnPoweredDown()
        {
            UpdatePowerIndicators();
        }

        private void Update()
        {
            HandleAnimation();
        }
        

        private void UpdatePowerIndicators()
        {
            var mat = this.IsPowered ? _powerOnMaterial : _powerOffMaterial;
            for (int mi = 0; mi < _powerIndicatorRenderers.Length; mi++)
            {
                _powerIndicatorRenderers[mi].material = mat;
            }
        }

        private void UpdateOpenIndicators()
        {
            var mat = this.IsOpen ? _openMaterial : _closedMaterial;
            for (int mi = 0; mi < _openIndicatorRenderers.Length; mi++)
            {
                _openIndicatorRenderers[mi].material = mat;
            }
        }

        private void HandleAnimation()
        {
            if (!_shouldAnimate || !this.IsPowered)
            {
                return;
            }

            if (!_animationIsOpening)
            {
                // If the door detects blockage while closing, then switch to be open.
                if (DoorObstructed())
                {
                    _signalNetwork.SetAndBroadcastSignal(true);
                }
            }

            float dp = Time.deltaTime / (_rotateDuration + _slideDuration);

            _animationOpenPercentage += _animationIsOpening ? dp : -dp;
            _animationOpenPercentage = Mathf.Clamp01(_animationOpenPercentage);
            
            SetAnimationOpenPercentage(_animationOpenPercentage);
            
            if ((_animationOpenPercentage >= 1f && _animationIsOpening) || (_animationOpenPercentage <= 0f && _animationIsOpening))
            {
                _shouldAnimate = false;
                
            }
        }

        private void SetAnimationOpenPercentage(float p)
        {
            float totalDuration = _rotateDuration + _slideDuration;
            float rd = _rotateDuration / totalDuration;
            float sd = _slideDuration / totalDuration;
            
            // Set rotation
            float rt = Mathf.Clamp01(p / rd);
            float rc = _rotateAnimationCurve.Evaluate(rt);
            float startAngle = 0f;
            float endAngle = Mathf.PI;

            var angle = Mathf.Lerp(startAngle, endAngle, rc) * Mathf.Rad2Deg;

            for (int ri = 0; ri < _rotators.Length; ri++)
            {
                var rotator = _rotators[ri];
                rotator.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            // Set slide
            float st = Mathf.Clamp01((p - rd) / sd);
            float sc = _slideAnimationCurve.Evaluate(st);

            float startPos = 0.0f;
            float endPos = _sideOpenDistance;

            var pos = Mathf.Lerp(startPos, endPos, sc);

            for (int si = 0; si < _sides.Length; si++)
            {
                var side = _sides[si];
                side.localPosition = side.forward * pos;
            }
        }

        private void BeginAnimation(bool opening)
        {
            _shouldAnimate = true;
            _animationIsOpening = opening;
        }

        private bool DoorObstructed()
        {
            int mask = ~LayerMask.GetMask("DoorSensorIgnore");

            for (int zi = 0; zi < _obstructionDetectionZones.Length; zi++)
            {
                var zone = _obstructionDetectionZones[zi];
                bool isObstructed = Physics.CheckBox(zone.position, zone.localScale * 0.5f, zone.rotation, mask);
                if (isObstructed)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnOpenSignalChanged(bool isOpen)
        {
            UpdateOpenIndicators();
            BeginAnimation(isOpen);
        }

        private void OnDrawGizmosSelected()
        {
            if (_obstructionDetectionZones != null)
            {
                Gizmos.color = new Color(247/255f, 114/255f, 12/255f);
                for (int zi = 0; zi < _obstructionDetectionZones.Length; zi++)
                {
                    var zone = _obstructionDetectionZones[zi];
                    if (zone == null)
                    {
                        continue;
                    }
                    // Note: This does not take rotation into account
                    Gizmos.DrawCube(zone.position, zone.localScale);
                }
            }
        }
    }
}