using CSafe.Commands.CsafeLogic;
using CSafe.Enums;

namespace CSafe.Commands.CSafe
{
    public class PaceCommand : Command
    {
        public uint Pace { get; private set; }
        public CSAFE_unit Unit { get; private set; }

        public PaceCommand() : base(CSAFE.GETPACE_CMD, 3)
        {
            Pace = 0;
            Unit = CSAFE_unit.unit_128_Undefined;
        }

        override protected void ReadInternal(ResponseReader reader)
        {
            Pace = reader.ReadUShort();
            Unit = (CSAFE_unit)reader.ReadByte();
        }

        override protected void WriteRest(CommandWriter writer)
        {
            // Nothing
        }

        public override string ToString()
        {
            return $"{GetType().Name} : Pace = {Pace} {CSafeUtil.convertCSafeUnitToText(Unit)}";
        }
    }
}