using System.IO;
using CSafe.Exceptions;

namespace CSafe.Commands
{
    public class CommandWriter
    {
        MemoryStream _ms = new MemoryStream();

        public CommandWriter()
        {
        }

        public byte[] Buffer => _ms.ToArray();

        public int Length => (int)_ms.Length;

        public void Reset()
        {
            _ms.SetLength(0);
        }

        public void WriteByte(byte val)
        {
            _ms.WriteByte(val);
        }
    }
}
