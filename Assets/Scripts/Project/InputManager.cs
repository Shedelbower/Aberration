using UnityEngine;

namespace Project
{
    public class InputManager : SingletonComponent<InputManager>
    {
        public float VerticalAxis { get; private set; }
        public float HorizontalAxis { get; private set; }
        
        
        public void Initialize()
        {
            
        }

        public void ResetInputs()
        {
            this.VerticalAxis = Input.GetAxisRaw("Vertical");
            this.HorizontalAxis = Input.GetAxisRaw("Horizontal");
        }

        private void LateUpdate()
        {
            // TODO: Put at start of frame and ensure this script runs first?
            ResetInputs();
        }
    }
}