using CSafe.Commands.CsafeLogic;
using CSafe.Enums;

namespace CSafe.Commands.CSafe
{
    public class PowerCommand : Command
    {
        public uint Power { get; private set; }
        public CSAFE_unit Unit { get; private set; }

        public PowerCommand() : base(CSAFE.GETPOWER_CMD, 3)
        {
            Power = 0;
            Unit = CSAFE_unit.unit_128_Undefined;
        }

        override protected void ReadInternal(ResponseReader reader)
        {
            Power = reader.ReadUShort();
            Unit = (CSAFE_unit)reader.ReadByte();
        }

        override protected void WriteRest(CommandWriter writer)
        {
            // Nothing
        }

        public override string ToString()
        {
            return $"{GetType().Name} : Power = {Power} {CSafeUtil.convertCSafeUnitToText(Unit)}";
        }
    }
}
