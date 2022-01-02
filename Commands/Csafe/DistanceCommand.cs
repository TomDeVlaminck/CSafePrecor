using CSafe.Commands.CsafeLogic;
using CSafe.Enums;

namespace CSafe.Commands.CSafe
{
    public class DistanceCommand : Command
    {
        public uint Distance { get; private set; }
        public CSAFE_unit Unit { get; private set; }

        public DistanceCommand() : base(CSAFE.GETHORIZONTAL_CMD, 3)
        {
            Distance = 0;
            Unit = CSAFE_unit.unit_128_Undefined;
        }

        override protected void ReadInternal(ResponseReader reader)
        {
            Distance = reader.ReadUShort();
            Unit = (CSAFE_unit)reader.ReadByte();
        }

        override protected void WriteRest(CommandWriter writer)
        {
            // Nothing
        }

        public override string ToString()
        {
            return $"{GetType().Name} : Distance = {Distance} {CSafeUtil.convertCSafeUnitToText(Unit)}";
        }
    }
}
