using UnityEngine;

namespace Project
{
    public class InputManager : SingletonComponent<InputManager>
    {
        public float VerticalAxis { get; private set; }
        public float HorizontalAxis { get; private set; }
        public float MouseX { get; private set; }
        public float MouseY { get; private set; }
        public bool ShiftKey { get; private set; }
        
        
        public void Initialize()
        {
            
        }

        public void UnlockMouse()
        {
            Cursor.lockState = CursorLockMode.None;
        }
        
        public void LockMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ResetInputs()
        {
            this.VerticalAxis = Input.GetAxisRaw("Vertical");
            this.HorizontalAxis = Input.GetAxisRaw("Horizontal");
            this.MouseX = Input.GetAxisRaw("Mouse X");
            this.MouseY = Input.GetAxisRaw("Mouse Y");
            this.ShiftKey = Input.GetKey(KeyCode.LeftShift);
        }

        private void LateUpdate()
        {
            // TODO: Put at start of frame and ensure this script runs first?
            ResetInputs();
        }
    }
}