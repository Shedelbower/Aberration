using System;
using UnityEngine;

namespace Project.Interactables
{
    public class SignalNetwork<T> : MonoBehaviour where T : IEquatable<T>
    {
        public delegate void SignalValueEvent(T value);
        public event SignalValueEvent OnSignalChanged;
        
        public T SignalValue => _signalValue;
        [SerializeField] private T _signalValue;

        public void SetAndBroadcastSignal(T value)
        {
            // Only broadcast signal if it changed
            if (_signalValue.Equals(value)) { return; }
            
            _signalValue = value;
            
            OnSignalChanged?.Invoke(_signalValue);
        }
    }
}