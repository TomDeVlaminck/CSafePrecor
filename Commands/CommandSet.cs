using System.Collections.Generic;
using System.Diagnostics;
using CSafe.Exceptions;

namespace CSafe.Commands
{
    public class CommandSet
    {
        private readonly List<Command> m_CSAFECommands;
        private CommandWriter m_CmdWriter;
        private bool m_Open;

        public bool CanAdd => m_Open;
        public bool CanSend => !m_Open;

        public CommandSet()
        {
            m_CSAFECommands = new List<Command>();
            m_CmdWriter = new CommandWriter();
            m_Open = true;
        }

        public void Reset()
        {
            m_CSAFECommands.Clear();
            m_CmdWriter.Reset();
            m_Open = true;
        }

        public byte[] getCommands()
        {
            return m_CmdWriter.Buffer;
        }

        public void Add(Command cmd)
        {
            if (m_Open)
            {
                m_CSAFECommands.Add(cmd);
            }
            else
                throw new CommandSetException("Cannot add new commands once set is prepared.");
        }

        public void Prepare()
        {
            if (m_Open)
            {
                // Custom commands
                // See original source -> https://github.com/b0urb4k1/Concept2-Rower
                // Add CSAFE commands
                foreach (Command cmd in m_CSAFECommands)
                    cmd.Write(m_CmdWriter);

                // Ensure no more commands are added
                m_Open = false;
            }
            else
            {
                throw new CommandSetException("Set is already prepared.");
            }
        }

        public bool Read(ResponseReader reader)
        {
            if (m_Open)
                throw new CommandSetException("Attempting to read set before it has been prepared.");

            var success = false;

            try
            {
                // [statusbyte][zero or more data structures]
                // [data structure] = [identifier, data byte count, data]

                byte statusByte = reader.ReadByte();
                // todo parse the status byte to something we can handle => https://web.archive.org/web/20060712183452/http://www.fitlinxx.com/csafe/Framework.htm

                // Read CSAFE commands
                foreach (Command cmd in m_CSAFECommands)
                    cmd.Read(reader);

                // Ensure whole response has been read
                success = (reader.Position == reader.Size);
            }
            catch (BufferExceededException e)
            {
                Debug.WriteLine(string.Format("[CommandSet.Read] {0}", e.Message));
            }

            return success;
        }
    }
}
