using UnityEngine;

namespace Project.Entities
{
    public class PlayableRover : PlayableEntity
    {
        [Header("Settings")]
        [SerializeField] private float _linearSpeed = 1.0f;
        [SerializeField] private float _rotateSpeed = 1.0f;
        
        
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
            Move(_inputBuffer.y);
            Turn(_inputBuffer.x);
            _inputBuffer = Vector2.zero;
        }

        private void Move(float speed)
        {
            float dist = speed * _linearSpeed * Time.deltaTime;
            this.transform.position += this.transform.forward * dist;
        }

        private void Turn(float speed)
        {
            float angle = speed * _rotateSpeed * Time.deltaTime;
            this.transform.rotation = Quaternion.AngleAxis(angle, this.transform.up) * this.transform.rotation;
        }
    }
}