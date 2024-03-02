using UnityEngine;

namespace Project.Interactables
{
    public class SignalNetwork : MonoBehaviour
    {
        public int SignalValue => _signalValue;

        [SerializeField] private int _signalValue;
        
        [SerializeField] private GameObject[] _recieverGameObjects;
        
        private ISignalReceiver[] _receivers;

        private void Awake()
        {
            Initialize(); // TODO: Handle initialization explicitly in a manager?
        }
        
        public void Initialize()
        {
            _receivers = new ISignalReceiver[_recieverGameObjects.Length];
            
            for (int ri = 0; ri < _receivers.Length; ri++)
            {
                _receivers[ri] = _recieverGameObjects[ri].GetComponent<ISignalReceiver>();
                if (_receivers[ri] == null)
                {
                    Debug.LogError($"Game object {_recieverGameObjects[ri].name} has no component that implements" +
                                   $"the interface ISignalReceiver");
                }
                _receivers[ri].SetInitialSignalValue(_signalValue);
            }
        }

        public void SetAndBroadcastSignal(int value)
        {
            if (_signalValue == value) { return; } // Only broadcast signal if it changed

            _signalValue = value;
            
            for (int ri = 0; ri < _receivers.Length; ri++)
            {
                _receivers[ri].OnSignalValueChanged(_signalValue);
            }
        }
    }
}