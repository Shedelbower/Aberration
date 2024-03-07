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
            GUI.Label(new Rect(10, 10, 700, 20), "Use arrow keys to move. \"C\" to crouch as quadruped");
            GUI.Label(new Rect(10, 30, 700, 20), "Q to toggle tablet (once picked up");
            GUI.Label(new Rect(10, 50, 700, 20), "Click on the fuse box to toggle lights");
            GUI.Label(new Rect(10, 70, 700, 20), "Note: Player movement temporarily broken with jumps");
        }
    }
}