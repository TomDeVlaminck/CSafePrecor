using CSafe.Commands.CsafeLogic;
using CSafe.Enums;

namespace CSafe.Commands.CSafe
{
    public class CadenceCommand : Command
    {
        public uint StrokeRate { get; private set; }
        public CSAFE_unit Unit { get; private set; }

        public CadenceCommand() : base(CSAFE.GETCADENCE_CMD, 3)
        {
            StrokeRate = 0;
            Unit = CSAFE_unit.unit_128_Undefined;
        }

        override protected void ReadInternal(ResponseReader reader)
        {
            StrokeRate = reader.ReadUShort();
            Unit = (CSAFE_unit)reader.ReadByte();
        }

        override protected void WriteRest(CommandWriter writer)
        {
            // Nothing
        }

        public override string ToString()
        {
            return $"{GetType().Name} : Cadence = {StrokeRate} {CSafeUtil.convertCSafeUnitToText(Unit)}";
        }
    }
}
