using Project.Entities;
using UnityEngine;

namespace Project.Interactables
{
    public class EntitySyncTerminal : PoweredInteractable
    {
        [Header("Settings")]
        [SerializeField] private Material _poweredIndicatorMaterial;
        [SerializeField] private Material _unpoweredIndicatorMaterial;
        
        [Header("Component References")]
        [SerializeField] private PlayableEntity[] _entitiesToSync;
        [SerializeField] private MeshRenderer _powerIndicatorRenderer;
        
        [Header("Debug")]
        [SerializeField] private bool _hasSynced;

        protected override void Initialize()
        {
            base.Initialize();

            foreach (var entity in _entitiesToSync)
            {
                entity.Initialize();
            }
        }

        protected override void SetInitialPoweredState(bool isPowered)
        {
            UpdateAppearance();
        }

        protected override void OnPoweredUp()
        {
            UpdateAppearance();
        }

        protected override void OnPoweredDown()
        {
            UpdateAppearance();
        }
        
        public override bool TryBeginInteraction()
        {
            if (!base.TryBeginInteraction()) { return false; }

            if (_hasSynced)
            {
                return false;
            }

            foreach (var entity in _entitiesToSync)
            {
                PlayerManager.Instance.SyncNewEntity(entity);
            }

            _hasSynced = true;
            
            return true;
        }

        private void UpdateAppearance()
        {
            var mat = this.IsPowered ? _poweredIndicatorMaterial : _unpoweredIndicatorMaterial;
            _powerIndicatorRenderer.material = mat;
        }
    }
}