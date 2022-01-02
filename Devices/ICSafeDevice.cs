namespace CSafe.Devices
{
    public interface ICSafeDevice
    {
        void Start();
        void Stop();
        int DiscoverUnits();
        byte[] SendCSAFECommand(byte[] cmdData);
    }
}