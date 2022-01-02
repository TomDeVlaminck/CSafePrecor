using CSafe.Enums;

namespace CSafe
{
    public interface IConnection
    {
        bool IsOpen { get; }
        ConnectionState State { get; }
        bool Open();
        void Close();
        byte[] SendCSAFECommand(byte[] cmdData);
    }
}
