using CSafe.Enums;

namespace CSafe.Commands.CSafe
{
    public class TimeCommand : Command
    {
        public uint TimeWorkHours { get; private set; }
        public uint TimeWorkMinutes { get; private set; }
        public uint TimeWorkSeconds { get; private set; }

        public TimeCommand() : base(CSAFE.GETTWORK_CMD, 3)
        {
            TimeWorkHours = 0;
            TimeWorkMinutes = 0;
            TimeWorkSeconds = 0;
        }

        override protected void ReadInternal(ResponseReader reader)
        {
            TimeWorkHours = reader.ReadByte();
            TimeWorkMinutes = reader.ReadByte();
            TimeWorkSeconds = reader.ReadByte();
        }

        override protected void WriteRest(CommandWriter writer)
        {
            // Nothing
        }

        public override string ToString()
        {
            return $"{GetType().Name} : Workout Time (Hours = {TimeWorkHours}, Minutes = {TimeWorkMinutes}, Seconds = {TimeWorkSeconds}";
        }
    }
}
