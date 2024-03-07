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
            GUI.skin.label.fontSize = 12;
            GUI.Label(new Rect(10, 10, 700, 20), "Use arrow keys to move. Press Q to toggle tablet view.");
            GUI.Label(new Rect(10, 30, 700, 20), "Use keys 1-3 to cycle through controllable entities");
            GUI.Label(new Rect(10, 50, 700, 20), "Clicking or E can be used to interact (depending on entity)");
        }
    }
}