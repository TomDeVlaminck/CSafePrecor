using CSafe.Enums;

namespace CSafe.Commands.CSafe
{
    public class HeartRateCommand : Command
    {
        public uint HeartRate { get; private set; }

        public HeartRateCommand() : base(CSAFE.GETHRCUR_CMD, 1)
        {
            HeartRate = 0;
        }

        override protected void ReadInternal(ResponseReader reader)
        {
            HeartRate = reader.ReadByte();
        }

        override protected void WriteRest(CommandWriter writer)
        {
            // Nothing
        }

        public override string ToString()
        {
            return $"{GetType().Name} : HeartRate = {HeartRate}";
        }
    }
}
