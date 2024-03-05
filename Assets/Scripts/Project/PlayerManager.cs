using Project.Entities;
using UnityEngine;

namespace Project
{
    public class PlayerManager : SingletonComponent<PlayerManager>
    {
        [Header("References")]
        [SerializeField] private PlayableEntity[] _playableEntities;
        
        private int _activeEntityIndex = -1;
        
        public void Initialize()
        {
            for (int ei = 0; ei < _playableEntities.Length; ei++)
            {
                _playableEntities[ei].Initialize();
            }

            SetActiveEntity(0);
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
        


        ////////////////////////////////////////////////////////////////////////
        //                          Unity Game Loop                           //
        ////////////////////////////////////////////////////////////////////////
        
        private void Update()
        {
            if (InputManager.Instance.Alpha1Down && _playableEntities.Length > 0)
            {
                SetActiveEntity(0);
            }
            else if (InputManager.Instance.Alpha2Down && _playableEntities.Length > 1)
            {
                SetActiveEntity(1);
            }
            else if (InputManager.Instance.Alpha3Down && _playableEntities.Length > 2)
            {
                SetActiveEntity(2);
            }
            else if (InputManager.Instance.Alpha4Down && _playableEntities.Length > 3)
            {
                SetActiveEntity(3);
            }
        }
        
    }
}