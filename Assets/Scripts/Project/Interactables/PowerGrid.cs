using UnityEngine;

namespace Project.Interactables
{
    public class PowerGrid : MonoBehaviour
    {
        public bool IsPowered => _isPowered;
        
        [SerializeField] private GameObject[] _poweredGameObjects;
        [SerializeField] private bool _isPowered;
        
        private IPowered[] _poweredEntities;

        private void Awake()
        {
            Initialize(); // TODO: Handle initialization explicitly in a manager?
        }
        
        public void Initialize()
        {
            _poweredEntities = new IPowered[_poweredGameObjects.Length];
            
            for (int pi = 0; pi < _poweredEntities.Length; pi++)
            {
                _poweredEntities[pi] = _poweredGameObjects[pi].GetComponent<IPowered>();
                if (_poweredEntities[pi] == null)
                {
                    Debug.LogError($"Game object {_poweredGameObjects[pi].name} has no component that implements" +
                                   $"the interface IPowered");
                }
                _poweredEntities[pi].SetInitialPoweredState(_isPowered);
            }
        }

        public void TogglePowered()
        {
            SetPowered(!_isPowered);
        }

        public void SetPowered(bool isPowered)
        {
            if (isPowered == _isPowered) { return; }
            
            _isPowered = isPowered;
            
            for (int pi = 0; pi < _poweredEntities.Length; pi++)
            {
                if (_isPowered)
                {
                    _poweredEntities[pi].OnPoweredUp();     
                }
                else
                {
                    _poweredEntities[pi].OnPoweredDown();     
                }
            }

            
        }
    }
}