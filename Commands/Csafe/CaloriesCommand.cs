using CSafe.Enums;

namespace CSafe.Commands.CSafe
{
    public class CaloriesCommand : Command
    {
        public uint Calories { get; private set; }

        public CaloriesCommand() : base(CSAFE.GETCALORIES_CMD, 2)
        {
            Calories = 0;
        }

        override protected void ReadInternal(ResponseReader reader)
        {
            Calories = reader.ReadUShort();
        }

        override protected void WriteRest(CommandWriter writer)
        {
            // Nothing
        }

        public override string ToString()
        {
            return $"{GetType().Name} : Calories = {Calories}";
        }
    }
}
