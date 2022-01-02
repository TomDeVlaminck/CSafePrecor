using System;
using CSafe.Exceptions;

namespace CSafe.Commands
{
    public class ResponseReader
    {
        private byte[] m_Buffer;
        private int m_Size;
        private int m_Position;

        public ushort Capacity => (ushort)m_Buffer.Length;
        public int Size => m_Size;
        public int Position => m_Position;
        public byte[] Buffer => m_Buffer;

        public ResponseReader(int capacity)
        {
            m_Buffer = new byte[capacity];
            m_Size = 0;
            m_Position = 0;
        }

        public void Reset(int size)
        {
            m_Size = Math.Min(size, m_Buffer.Length);
            m_Position = 0;
        }

        public byte ReadByte()
        {
            if (m_Position >= m_Size)
            {
                throw new BufferExceededException("Attempted to read past end of response buffer.");
            }

            return m_Buffer[m_Position++];
        }

        public uint ReadUShort()
        {
            uint val = ((uint) ReadByte() << 0) + ((uint) ReadByte() << 8);
            return val;
        }

        public uint ReadUInt()
        {
            uint val = ((uint)ReadByte() << 0) + ((uint)ReadByte() << 8) + ((uint)ReadByte() << 16) + ((uint)ReadByte() << 24);
            return val;
        }
    }
}
