using System;
using UnityEngine;

namespace Project.Entities
{
    public abstract class PlayableEntity : MonoBehaviour
    {
        public bool IsActive { get; set; }
        
        // TODO: Handle logic for when/if entity destroyed

        [SerializeField] protected Camera _camera;

        public virtual void Initialize()
        {
            this.IsActive = false;
            _camera.enabled = false;
        }

        public virtual void OnActivated()
        {
            if (this.IsActive) { return; }
            this.IsActive = true;
            _camera.enabled = true;
        }
        
        public virtual void OnDeactivated()
        {
            if (!this.IsActive) { return; }
            this.IsActive = false;
            _camera.enabled = false;
        }
        
    }
}