using System.Linq;
using Project.Entities;
using Project.Interactables;
using UnityEngine;

namespace Project
{
    public class PlayerManager : SingletonComponent<PlayerManager>
    {
        [Header("References")]
        [SerializeField] private PlayableEntity _defaultEntity;
        [SerializeField] private PlayableEntity[] _remoteEntities;
        [SerializeField] private Tablet _tablet;
        
        private int _prevRemoteEntityIndex = -1;
        private int _activeRemoteEntityIndex = -1;
        
        public void Initialize()
        {
            _defaultEntity.Initialize();
            
            for (int ei = 0; ei < _remoteEntities.Length; ei++)
            {
                _remoteEntities[ei].Initialize();
            }

            SetDefaultEntityActive();
        }

        public void SyncNewEntity(PlayableEntity entity)
        {
            _remoteEntities = _remoteEntities.Append(entity).ToArray();
        }

        private void SetDefaultEntityActive()
        {
            if (_defaultEntity.IsActive) { return; }

            if (_activeRemoteEntityIndex >= 0)
            {
                _prevRemoteEntityIndex = _activeRemoteEntityIndex;
                _remoteEntities[_activeRemoteEntityIndex].OnDeactivated();
                _remoteEntities[_activeRemoteEntityIndex].IsActive = false;
                _activeRemoteEntityIndex = -1;
            }
            
            _defaultEntity.OnActivated();
            _defaultEntity.IsActive = true;
        }
        
        private void SetRemoteEntityActive(int index)
        {
            if (_activeRemoteEntityIndex == index) { return; }
            
            if (_defaultEntity.IsActive)
            {
                _defaultEntity.OnDeactivated();
                _defaultEntity.IsActive = false;
            }
            
            if (!_tablet.TabletIsRaised)
            {
                RaiseTablet();
            }

            if (_activeRemoteEntityIndex >= 0)
            {   
                // Deactivate old remote entity
                _prevRemoteEntityIndex = _activeRemoteEntityIndex;
                _remoteEntities[_activeRemoteEntityIndex].OnDeactivated();
                _remoteEntities[_activeRemoteEntityIndex].IsActive = false;
                _activeRemoteEntityIndex = -1;
            }
            
            // Activate new entity
            _activeRemoteEntityIndex = index;
            _remoteEntities[_activeRemoteEntityIndex].OnActivated();
            _remoteEntities[_activeRemoteEntityIndex].IsActive = true;
        }

        private void RestorePreviousRemoteEntity()
        {
            SetRemoteEntityActive(_prevRemoteEntityIndex);
        }
        
        private void ToggleTabletVisibility()
        {
            if (_tablet.TabletIsRaised)
            {
                LowerTablet();
            }
            else
            {
                RaiseTablet();
            }
        }

        private void RaiseTablet()
        {
            _tablet.BeginRaiseAnimation();
            RestorePreviousRemoteEntity();
        }
        
        private void LowerTablet()
        {
            _tablet.BeginLowerAnimation();
            SetDefaultEntityActive();
        }
        

        ////////////////////////////////////////////////////////////////////////
        //                          Unity Game Loop                           //
        ////////////////////////////////////////////////////////////////////////
        
        private void Update()
        {

            if (_tablet.TabletIsHeld)
            {
                if (InputManager.Instance.QDown)
                {
                    ToggleTabletVisibility();
                }
            
                if (InputManager.Instance.Alpha1Down && _remoteEntities.Length > 0)
                {
                    SetRemoteEntityActive(0);
                }
                else if (InputManager.Instance.Alpha2Down && _remoteEntities.Length > 1)
                {
                    SetRemoteEntityActive(1);
                }
                else if (InputManager.Instance.Alpha3Down && _remoteEntities.Length > 2)
                {
                    SetRemoteEntityActive(2);
                }
                else if (InputManager.Instance.Alpha4Down && _remoteEntities.Length > 3)
                {
                    SetRemoteEntityActive(3);
                }    
            }
            
        }
        
        
    }
}