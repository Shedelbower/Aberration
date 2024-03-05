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
        public bool Space { get; private set; }
        
        
        public bool Alpha1Down { get; private set; }
        public bool Alpha2Down { get; private set; }
        public bool Alpha3Down { get; private set; }
        public bool Alpha4Down { get; private set; }
        
        public bool SpaceDown { get; private set; }
        public bool EDown { get; private set; }
        public bool LeftMouseDown { get; private set; }
        
        public bool SpaceDownPersist { get; set; }
        
        
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
            this.Space = Input.GetKey(KeyCode.Space);

            this.Alpha1Down = Input.GetKeyDown(KeyCode.Alpha1);
            this.Alpha2Down = Input.GetKeyDown(KeyCode.Alpha2);
            this.Alpha3Down = Input.GetKeyDown(KeyCode.Alpha3);
            this.Alpha4Down = Input.GetKeyDown(KeyCode.Alpha4);

            this.SpaceDown = Input.GetKeyDown(KeyCode.Space);
            this.EDown = Input.GetKeyDown(KeyCode.E);
            this.LeftMouseDown = Input.GetMouseButtonDown(0);

            this.SpaceDownPersist |= this.SpaceDown;
        }

        private void LateUpdate()
        {
            // TODO: Put at start of frame and ensure this script runs first?
            ResetInputs();
        }
    }
}