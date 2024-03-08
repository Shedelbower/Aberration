using UnityEngine;

namespace Project.Entities
{
    public abstract class PlayableEntity : MonoBehaviour
    {
        public bool IsActive { get; set; }
        public bool CameraIsRendering { get; set; }
        
        // TODO: Handle logic for when/if entity destroyed

        [SerializeField] protected bool _isDefaultEntity;
        [SerializeField] protected Camera _camera;

        public virtual void Initialize()
        {
            this.IsActive = false;
            _camera.enabled = _isDefaultEntity;   
        }

        public virtual void OnActivated()
        {
            if (this.IsActive) { return; }
            this.IsActive = true;
        }
        
        public virtual void OnDeactivated()
        {
            if (!this.IsActive) { return; }
            this.IsActive = false;
        }
        
        public virtual void OnCameraStartRendering()
        {
            if (this.CameraIsRendering) { return; }
            this.CameraIsRendering = true;
            _camera.enabled = true;
        }
        
        public virtual void OnCameraStopRendering()
        {
            if (!this.CameraIsRendering || _isDefaultEntity) { return; }
            this.CameraIsRendering = false;
            _camera.enabled = false;
        }
        
    }
}