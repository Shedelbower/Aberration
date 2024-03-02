using UnityEngine;

namespace Project.Interactables
{
    public interface IPowered
    {
        public void OnPoweredUp();

        public void OnPoweredDown();

        public void SetInitialPoweredState(bool isPowered);
    }
}