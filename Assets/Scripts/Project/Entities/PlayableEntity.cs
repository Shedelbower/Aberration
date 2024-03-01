using System;
using UnityEngine;

namespace Project.Entities
{
    public abstract class PlayableEntity : MonoBehaviour
    {
        // TODO: Handle logic for when/if entity destroyed

        [SerializeField] protected Camera _camera;


        public virtual void Initialize()
        {
            _camera.enabled = false;
        }

        public virtual void OnPlayingStart()
        {
            _camera.enabled = true;
        }
        
        public virtual void OnPlayingEnd()
        {
            _camera.enabled = false;
        }
        
        public virtual void OnUpdate() { }
        
        public virtual void OnFixedUpdate() { }
        
    }
}