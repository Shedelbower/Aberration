namespace Project.Interactables
{
    public interface ISignalReceiver
    {
        public void OnSignalValueChanged(int value);
        public void SetInitialSignalValue(int value);
    }
}