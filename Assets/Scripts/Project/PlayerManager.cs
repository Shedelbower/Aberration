using System.Linq;
using Project.Entities;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace Project
{
    public class PlayerManager : SingletonComponent<PlayerManager>
    {
        [Header("References")]
        [SerializeField] private PlayableEntity _defaultEntity;
        [SerializeField] private PlayableEntity[] _optionalEntities;
        
        private PlayableEntity[] _playableEntities;

        public bool TabletIsShown => _tabletShown;
        
        private int _defaultEntityIndex = 0;
        private int _prevOptionalEntityIndex = 1;
        private int _activeEntityIndex = -1;

        private bool _tabletShown;
        
        public void Initialize()
        {
            _playableEntities = _optionalEntities.Prepend(_defaultEntity).ToArray();
            
            for (int ei = 0; ei < _playableEntities.Length; ei++)
            {
                _playableEntities[ei].Initialize();
            }

            SetActiveEntity(_defaultEntityIndex);
        }

        private void SetActiveEntity(int activeIndex)
        {
            if (_playableEntities.Length == 0)
            {
                Debug.LogWarning("No playable entities registered");
                return;
            }
            
            if (activeIndex == _activeEntityIndex)
            {
                Debug.LogWarning("Setting active playable entity to current entity");
                return;
            }

            if (_activeEntityIndex >= 0) // Handle start up case where activeIndex = -1
            {
                _playableEntities[_activeEntityIndex].OnDeactivated();
                _playableEntities[_activeEntityIndex].IsActive = false;
            }
            
            _activeEntityIndex = activeIndex;
            _playableEntities[_activeEntityIndex].OnActivated();
            _playableEntities[_activeEntityIndex].IsActive = true;
        }

        private void SwapActiveEntity()
        {
            int newIndex = (_activeEntityIndex + 1) % _playableEntities.Length;
            SetActiveEntity(newIndex);
        }
        
        private void ToggleTabletVisibility()
        {
            if (_tabletShown)
            {
                HideTablet();
            }
            else
            {
                ShowTablet();
            }
        }

        private void ShowTablet()
        {
            var nextIndex = _prevOptionalEntityIndex;
            SetActiveEntity(nextIndex);
            _tabletShown = true;
        }
        
        private void HideTablet()
        {
            _prevOptionalEntityIndex = _activeEntityIndex;
            SetActiveEntity(_defaultEntityIndex);
            _tabletShown = false;
        }

        ////////////////////////////////////////////////////////////////////////
        //                          Unity Game Loop                           //
        ////////////////////////////////////////////////////////////////////////
        
        private void Update()
        {
            if (InputManager.Instance.QDown)
            {
                ToggleTabletVisibility();
            }
            
            if (InputManager.Instance.Alpha1Down && _playableEntities.Length > 1)
            {
                SetActiveEntity(1);
            }
            else if (InputManager.Instance.Alpha2Down && _playableEntities.Length > 2)
            {
                SetActiveEntity(2);
            }
            else if (InputManager.Instance.Alpha3Down && _playableEntities.Length > 3)
            {
                SetActiveEntity(3);
            }
            else if (InputManager.Instance.Alpha4Down && _playableEntities.Length > 4)
            {
                SetActiveEntity(4);
            }
        }
        
        
    }
}