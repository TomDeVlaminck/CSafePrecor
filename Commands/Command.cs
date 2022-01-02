using System.Diagnostics;
using CSafe.Enums;

namespace CSafe.Commands
{
    public abstract class Command
    {
        private CSAFE m_Id;
        private readonly byte m_RspSize;

        abstract protected void ReadInternal(ResponseReader reader);
        abstract protected void WriteRest(CommandWriter writer);

        protected Command(CSAFE id, byte rspSize)
        {
            m_Id = id;
            m_RspSize = rspSize;
        }

        public void Write(CommandWriter writer)
        {
            // the first byte is always the command
            writer.WriteByte((byte) m_Id);
            // the optional next bytes are for commands with extra arguments
            WriteRest(writer);
        }

        public void Read(ResponseReader reader)
        {
            byte id = reader.ReadByte();
            byte size = reader.ReadByte();

            if (id == (byte)m_Id && size == m_RspSize)
                ReadInternal(reader);
            else
                Debug.WriteLine("[Command.Read] id/size mismatch");
        }
    }
}
