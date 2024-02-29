using UnityEngine;

namespace Project.Entities
{
    public class PlayableSecurityCamera : PlayableEntity
    {
        [SerializeField] private float _rotateSpeed = 10.0f;
        
        private Vector2 _inputBuffer;
        
        public override void OnUpdate()
        {
            _inputBuffer = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
        }

        public override void OnFixedUpdate()
        {
            // TODO: Have vertical axis control zoom (i.e. field of view).
            ChangeRotation(_inputBuffer.x, _inputBuffer.y);
            _inputBuffer = Vector2.zero;
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