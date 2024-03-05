using UnityEngine;

namespace Project.Entities
{
    public class PlayableSecurityCamera : PlayableEntity
    {
        [SerializeField] private float _rotateSpeed = 10.0f;
        
        
        // private void Update()
        // {
        //     if (!this.IsActive) { return; }
        // }

        private void FixedUpdate()
        {
            if (!this.IsActive) { return; }
            
            // TODO: Have vertical axis control zoom (i.e. field of view).
            ChangeRotation(InputManager.Instance.HorizontalAxis, InputManager.Instance.VerticalAxis);
        }

        private void ChangeRotation(float horizontal, float vertical)
        {
            
            float ah = horizontal * _rotateSpeed * Time.fixedDeltaTime;
            float av = vertical * _rotateSpeed * Time.fixedDeltaTime;
            
            var rot = Quaternion.AngleAxis(ah, this.transform.up) * Quaternion.AngleAxis(av, -this.transform.right);

            this.transform.rotation = rot * this.transform.rotation;
        }
        
    }
}