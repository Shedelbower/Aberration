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

        private bool _showingInteractionUI = false;
        private Interactable _currentUIInteractable;
        private int _interactionUIFrameCountDown;
        
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
                _activeRemoteEntityIndex = -1;
            }
            
            _defaultEntity.OnActivated();
            if (!_defaultEntity.CameraIsRendering)
            {
                _defaultEntity.OnCameraStartRendering();
            }
        }
        
        private void SetRemoteEntityActive(int index)
        {
            if (_activeRemoteEntityIndex == index) { return; }
            
            if (_defaultEntity.IsActive)
            {
                // Deactivate player controls, but keep camera feed active
                _defaultEntity.OnDeactivated();
            }
            
            if (!_tablet.TabletIsRaised)
            {
                RaiseTablet();
            }

            PlayableEntity entity;

            if (_activeRemoteEntityIndex >= 0)
            {   
                // Deactivate old remote entity
                _prevRemoteEntityIndex = _activeRemoteEntityIndex;
                entity = _remoteEntities[_activeRemoteEntityIndex];
                entity.OnDeactivated();
                entity.OnCameraStopRendering();
                _activeRemoteEntityIndex = -1;
            }
            
            // Activate new entity
            _activeRemoteEntityIndex = index;
            entity = _remoteEntities[_activeRemoteEntityIndex];
            entity.OnActivated();
            entity.OnCameraStartRendering();
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
        
        public void ShowInteractionUIFor(Interactable interactable)
        {
            _showingInteractionUI = true;
            _currentUIInteractable = interactable;
            _interactionUIFrameCountDown = 2; // Hacky way to disable UI once the entity gets out of range.
        }

        private void HideInteractionUI()
        {
            _showingInteractionUI = false;
            _currentUIInteractable = null;
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
        
        private void LateUpdate()
        {
            if (_showingInteractionUI)
            {
                _interactionUIFrameCountDown--;
                if (_interactionUIFrameCountDown <= 0)
                {
                    HideInteractionUI();
                }
            }
        }
        
        
    }
}