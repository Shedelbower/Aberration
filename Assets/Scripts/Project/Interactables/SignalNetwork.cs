using System.Collections.Generic;
using UnityEngine;

namespace Project.Interactables
{
    public class SignalNetwork : MonoBehaviour
    {
        public int SignalValue => _signalValue;

        [SerializeField] private int _signalValue;
        
        private List<ISignalReceiver> _receivers;

        private bool _initialized = false;
        
        public void Initialize()
        {
            _initialized = true;
            _receivers = new List<ISignalReceiver>();
        }

        public void RegisterReceiver(ISignalReceiver receiver)
        {
            if (!_initialized) { Initialize(); }
            
            _receivers.Add(receiver);
            receiver.SetInitialSignalValue(_signalValue);
        }

        public void SetAndBroadcastSignal(int value)
        {
            if (!_initialized) { Initialize(); }
            
            if (_signalValue == value) { return; } // Only broadcast signal if it changed

            _signalValue = value;
            
            for (int ri = 0; ri < _receivers.Count; ri++)
            {
                _receivers[ri].OnSignalValueChanged(_signalValue);
            }
        }
    }
}