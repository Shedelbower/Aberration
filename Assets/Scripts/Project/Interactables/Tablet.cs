using System;
using Project.Entities;
using UnityEngine;

namespace Project.Interactables
{
    public class Tablet : Interactable
    {
        [Header("Settings")]
        [SerializeField] private float _movementDuration = 1.0f;
        [SerializeField] private AnimationCurve _raiseAnimationCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("Component References")]
        [SerializeField] private Collider _collider;
        [SerializeField] private PlayablePerson _person;

        public bool TabletIsRaised => (_tabletAnimationState == TabletAnimationState.Raised ||
                                       _tabletAnimationState == TabletAnimationState.Raising);

        public bool TabletIsHeld => _isHeld;

        private bool _isHeld;
        
        private enum TabletAnimationState
        {
            Raised,
            Lowered,
            Raising,
            Lowering
        }

        private TabletAnimationState _tabletAnimationState;
        
        private Transform _loweredTransform;
        private Transform _raisedTransform;

        private float _movementTimer;

        private Quaternion _startRotation;
        private Vector3 _startPosition;
        
        public void AssignTransforms(Transform lowered, Transform raised)
        {
            _isHeld = true;
            _loweredTransform = lowered;
            _raisedTransform = raised;
        }
        
        public override bool TryBeginInteraction()
        {
            _collider.enabled = false; // Prevent selecting after being picked up the first time
            _person.OnPickupTablet(this);
            return true;
        }

        public void BeginLowerAnimation()
        {
            _startRotation = this.transform.rotation;
            _startPosition = this.transform.position;
            _tabletAnimationState = TabletAnimationState.Lowering;
        }
        
        public void BeginRaiseAnimation()
        {
            _startRotation = this.transform.rotation;
            _startPosition = this.transform.position;
            _tabletAnimationState = TabletAnimationState.Raising;
        }

        private void Update()
        {
            UpdateAnimation();
        }
        
        private void UpdateAnimation()
        {
            if (_tabletAnimationState == TabletAnimationState.Raising || _tabletAnimationState == TabletAnimationState.Lowering)
            {
                _movementTimer += Time.deltaTime;
                var end = _tabletAnimationState == TabletAnimationState.Raising
                    ? _raisedTransform
                    : _loweredTransform;
                float s = Mathf.Clamp01(_movementTimer / _movementDuration);
                var t = _raiseAnimationCurve.Evaluate(s);

                this.transform.position = Vector3.LerpUnclamped(_startPosition, end.position, t);
                this.transform.rotation = Quaternion.LerpUnclamped(_startRotation, end.rotation, t);
                if (s >= 1f)
                {
                    if (_tabletAnimationState == TabletAnimationState.Lowering)
                    {
                        _tabletAnimationState = TabletAnimationState.Lowered;
                        this.transform.parent = _loweredTransform;
                        _movementTimer = 0.0f;
                    }
                    else
                    {
                        _tabletAnimationState = TabletAnimationState.Raised;
                        this.transform.parent = _raisedTransform;
                        _movementTimer = 0.0f;
                    }
                }
            }
        }
    }
}