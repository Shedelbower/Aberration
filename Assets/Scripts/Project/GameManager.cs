using System;
using Project.Entities;
using UnityEngine;

namespace Project
{
    public class GameManager : SingletonComponent<GameManager>
    {
        [Header("Managers")]
        [SerializeField] private CameraManager _cameraManager;
        
        [Header("References")]
        [SerializeField] private PlayableEntity[] _playableEntities;

        private int _activeEntityIndex = -1;
        
        private void Initialize()
        {
            _cameraManager.Initialize();

            for (int ei = 0; ei < _playableEntities.Length; ei++)
            {
                _playableEntities[ei].Initialize();
            }

            SetActiveEntity(0);
        }

        private void SetActiveEntity(int activeIndex)
        {
            if (activeIndex == _activeEntityIndex)
            {
                return;
            }

            if (activeIndex > 0) // Handle start up case where activeIndex = -1
            {
                _playableEntities[_activeEntityIndex].OnPlayingEnd();
            }
            
            _activeEntityIndex = activeIndex;
            _playableEntities[_activeEntityIndex].OnPlayingStart();
        }

        private void SwapActiveEntity()
        {
            int newIndex = (_activeEntityIndex + 1) % _playableEntities.Length;
            SetActiveEntity(newIndex);
        }
        


        ////////////////////////////////////////////////////////////////////////
        //                          Unity Game Loop                           //
        ////////////////////////////////////////////////////////////////////////
        
        
        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SwapActiveEntity();
            }
            
            _playableEntities[_activeEntityIndex].OnUpdate();
        }

        private void FixedUpdate()
        {
            _playableEntities[_activeEntityIndex].OnFixedUpdate();
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 500, 20), "Use arrow keys to move and space to swap entities");
            GUI.Label(new Rect(10, 30, 500, 20), "(Rover physics not implemented yet)");
        }
    }
}