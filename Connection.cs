using System;
using System.Diagnostics;
using CSafe.Devices;
using CSafe.Enums;

namespace CSafe
{
    public class Connection : IConnection
    {
        public Connection()
        {
            // CSafeSer2Net device type (precor)
            m_CSafeDevice = new Devices.CSafeSer2Net();

            m_State = ConnectionState.Disconnected;

            m_CSafeDevice.Start();
        }

        public bool IsOpen => (m_State == ConnectionState.Connected || m_State == ConnectionState.SendError);

        public ConnectionState State => m_State;

        public bool Open()
        {
            if (m_State == ConnectionState.Disconnected)
            {
                int numUnits = m_CSafeDevice.DiscoverUnits();
                if (numUnits > 0)
                {
                    try
                    {
                        m_State = ConnectionState.Connected;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(string.Format("[Connection.Open] {0}", e.Message));
                    }
                }
            }
            else
            {
                Debug.WriteLine("[Connection.Open] Connection already open");
            }

            return IsOpen;
        }

        public void Close()
        {
            m_State = ConnectionState.Disconnected;
        }

        public byte[] SendCSAFECommand(byte[] cmdData)
        {
            if (IsOpen)
            {
                return m_CSafeDevice.SendCSAFECommand(cmdData);
            }
            throw new System.Exception("Connection not open");
        }

        private ICSafeDevice m_CSafeDevice;
        private ConnectionState m_State;
    }
}
