using System;
using Project.Entities;
using UnityEngine;

namespace Project
{
    public class GameManager : SingletonComponent<GameManager>
    {
        [Header("Managers")]
        [SerializeField] private CameraManager _cameraManager;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private LevelManager _levelManager;

        
        private void Initialize()
        {
            _cameraManager.Initialize();
            _inputManager.Initialize();
            _playerManager.Initialize();
            _levelManager.Initialize();
        }

        ////////////////////////////////////////////////////////////////////////
        //                          Unity Game Loop                           //
        ////////////////////////////////////////////////////////////////////////
        
        
        private void Awake()
        {
            Initialize();
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