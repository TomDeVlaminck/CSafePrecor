// CSafe Ser2Net => A precor elliptical trainer connected via CSAFE port (slave) <-> espeasy (ser2net) (hostname:port) (server) <-> this program (client)
// (c) 31/12/2021, Tom De Vlaminck (tom_de_vlaminck@hotmail.com)

using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace CSafe.Devices
{
    public class CSafeSer2Net : ICSafeDevice
    {
        private readonly string _espEasyHostName = "dr-tom-espeasy-40";
        private readonly int _espEasySer2NetPort = 1234;
        private TcpClient _tcpClient;

        public void Start()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                // socket is connected, first disconnect
                _tcpClient.Close();
            }

            if (_tcpClient == null)
            {
                _tcpClient = new TcpClient(_espEasyHostName, _espEasySer2NetPort);
            }
            else
            {
                // Setup client socket to ser2net on espeasy esp8266 device
                _tcpClient.Connect(_espEasyHostName, _espEasySer2NetPort);
            }
            // When the connection succeeds we can send CSAFE commands as if it was a serial direct connection

        }

        public void Stop()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                // socket is connected, first disconnect
                _tcpClient.Close();
            }
        }

        public int DiscoverUnits()
        {
            // If we are connected we can assume 1 active unit is discovered
            return (_tcpClient != null && _tcpClient.Connected) ? 1 : 0;
        }

        public byte[] SendCSAFECommand(byte[] cmdData)
        {
            // Get a client stream for reading and writing.
            NetworkStream stream = _tcpClient.GetStream();
            // Send the message to the connected TcpServer.
            stream.Write(cmdData, 0, cmdData.Length);
            Console.WriteLine("Sent: {0}", BitConverter.ToString(cmdData).Replace('-', ' '));
            // Receive the TcpServer.response.
            // Read the first batch of the TcpServer response bytes.
            byte[] buffer = new byte[1024];
            int nmbrBytesResponded = stream.Read(buffer, 0, buffer.Length);
            MemoryStream ms = new MemoryStream();
            ms.Write(buffer, 0, nmbrBytesResponded);
            byte[] respData = ms.ToArray();
            Console.WriteLine("Received: {0}", BitConverter.ToString(respData).Replace('-', ' '));
            // Close everything.
            stream.Close();
            return respData;
        }
    }
}