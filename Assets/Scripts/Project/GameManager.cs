using System;
using Project.Entities;
using UnityEngine;

namespace Project
{
    public class GameManager : SingletonComponent<GameManager>
    {
        [Header("Managers")]
        [SerializeField] private CameraManager _cameraManager;
        [SerializeField] private LevelManager _levelManager;
        
        [Header("References")]
        [SerializeField] private PlayableEntity[] _playableEntities;

        private int _activeEntityIndex = -1;
        
        private void Initialize()
        {
            _cameraManager.Initialize();
            _levelManager.Initialize();

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
            GUI.Label(new Rect(10, 10, 700, 20), "Use arrow keys to move. Losing ground on all legs will trigger a ragdoll state");
            GUI.Label(new Rect(10, 30, 600, 20), "This is a test scene for the new quadruped physics and animations");
            GUI.Label(new Rect(10, 50, 500, 20), "Slowly getting better...");
            // GUI.Label(new Rect(10, 10, 500, 20), "Use arrow keys to move and space to swap entities");
            // GUI.Label(new Rect(10, 30, 600, 20), "E to interact with switches and LShift to \"cloak\" white cylinder");
            // GUI.Label(new Rect(10, 50, 500, 20), "(Rover physics and IK implemented, but not great...)");
        }
    }
}