using System;
using System.IO;
using CSafe.Enums;

namespace CSafe.Commands.CsafeLogic
{
    static public class CSafeUtil
    {
        public const byte CSAFE_START_FLAG = 0xf1;
        public const byte CSAFE_STOP_FLAG = 0xf2;

        /// Checksum computes a single byte with exclusive OR on
        /// all CSafeCommand in the slice.
        private static byte checksum(byte[] data)
        {
            byte checksum = 0;
            for (int i = 0; i < data.Length; i++)
                checksum = (byte)(data[i] ^ checksum);
            return checksum;
        }

        /// stuff_bytes is CSAFE's response to the fact that its start and end flags
        /// could very well be actual data - it's very possible that '0xf0' could be
        /// some meter total or whatever. As a result, whenever one of these bytes
        /// is in data, the byte is replaced with *two* bytes - '0xf3' and another
        /// byte. This is also done in reverse from the Concept2 machine; the response
        /// will have to be *unstuffed*.
        private static byte[] stuffBytes(byte[] unstuffedBytes)
        {
            MemoryStream stuffedMs = new MemoryStream();
            foreach (byte b in unstuffedBytes)
            {
                switch (b)
                {
                    case 0xf0:
                        stuffedMs.WriteByte(0xf3);
                        stuffedMs.WriteByte(0x00);
                        break;
                    case 0xf1:
                        stuffedMs.WriteByte(0xf3);
                        stuffedMs.WriteByte(0x01);
                        break;
                    case 0xf2:
                        stuffedMs.WriteByte(0xf3);
                        stuffedMs.WriteByte(0x02);
                        break;
                    case 0xf3:
                        stuffedMs.WriteByte(0xf3);
                        stuffedMs.WriteByte(0x003);
                        break;
                    default:
                        stuffedMs.WriteByte(b);
                        break;
                }
            }
            return stuffedMs.ToArray();
        }

        private static byte[] unstuffBytes(byte[] stuffedBytes)
        {
            MemoryStream unstuffedMs = new MemoryStream();
            int i = 0;
            while (i < stuffedBytes.Length)
            {
                byte val = stuffedBytes[i];
                if (val == 0xf3)
                {
                    val = (byte)(0xf0 | stuffedBytes[i + 1]);
                    i += 2;
                }
                else
                {
                    i++;
                }
                unstuffedMs.WriteByte(val);
            }
            return unstuffedMs.ToArray();
        }

        static public byte[] checkAndRemoveCheckSum(byte[] csafeResponseUnstuffedWithCRC)
        {
            // Recalculate the checksum. this must match with the checksum in the response.
            byte responseChecksum = csafeResponseUnstuffedWithCRC[csafeResponseUnstuffedWithCRC.Length - 1];

            MemoryStream responseDataMs = new MemoryStream();
            responseDataMs.Write(csafeResponseUnstuffedWithCRC, 0, csafeResponseUnstuffedWithCRC.Length - 1);
            byte[] responseData = responseDataMs.ToArray();

            byte responseCalculatedChecksum = checksum(responseData);

            if (responseChecksum != responseCalculatedChecksum)
                throw new Exception($"Checksum in response {responseChecksum} doesn't match the calculated checksum {responseCalculatedChecksum}");

            return responseData;
        }


        static public byte[] generateCSAFEFrame(byte[] csafecommands)
        {
            // the command objects can write themselves to a byte stream
            // we must calculate the checksum of the bytes contained in this stream
            // we must stuff certain bytes on (bytes in stream + crc)
            // then the start and stop must be added
            // so we get start + StuffBytes (((command1 bytes)...(commandn bytes)) + CRC(((command1 bytes)...(commandn bytes)))) + stop
            // the bytes can than be send to the CSAFE device
            MemoryStream cSafeFrameWithCRC = new MemoryStream();
            cSafeFrameWithCRC.Write(csafecommands, 0, csafecommands.Length);
            cSafeFrameWithCRC.WriteByte(checksum(csafecommands));
            byte[] cSafeFrameWithCRCAndStuffed = stuffBytes(cSafeFrameWithCRC.ToArray());
            MemoryStream cSafeFrameWithCRCStuffedWithStartStop = new MemoryStream();
            cSafeFrameWithCRCStuffedWithStartStop.WriteByte(CSAFE_START_FLAG);
            cSafeFrameWithCRCStuffedWithStartStop.Write(cSafeFrameWithCRCAndStuffed, 0, cSafeFrameWithCRCAndStuffed.Length);
            cSafeFrameWithCRCStuffedWithStartStop.WriteByte(CSAFE_STOP_FLAG);
            return cSafeFrameWithCRCStuffedWithStartStop.ToArray();
        }

        // find start and  stop marker (stop must be after te start position)
        // remove start and stop marker

        static public byte[] extractCSAFEResponse(byte[] csafeResponseFrame)
        {
            // the commands can read the responses data but first
            // we need to check the frame for start en end marker (end must be after start marker)
            // then we need to unstuff the data
            // then we need to check the crc and remove it from the data
            // then we have the response frame
            // the response frame consists of the status byte + 0 or more data structures
            // the data structures can be parsed by the command objects
            int startMarkerPos = Array.IndexOf(csafeResponseFrame, CSAFE_START_FLAG);
            int stopMarkerPos = Array.IndexOf(csafeResponseFrame, CSAFE_STOP_FLAG);
            if (startMarkerPos == -1 || stopMarkerPos == -1)
                throw new Exception("No start or end marker found in response");
            if (stopMarkerPos < startMarkerPos)
                throw new Exception("End marker found before start marker in response");
            MemoryStream csafeResponseFrameStuffed = new MemoryStream();
            csafeResponseFrameStuffed.Write(csafeResponseFrame, startMarkerPos + 1, stopMarkerPos - startMarkerPos - 1);
            byte[] csafeResponseFrameUnstuffed = unstuffBytes(csafeResponseFrameStuffed.ToArray());
            // Now check the checksum
            byte[] csafeResponseVerified = checkAndRemoveCheckSum(csafeResponseFrameUnstuffed);
            return csafeResponseVerified;
        }

        static public string convertCSafeUnitToText(CSAFE_unit unit)
        {
            switch (unit)
            {
                case CSAFE_unit.unit_1_Mile: return "mile";
                case CSAFE_unit.unit_2_0_1mile: return "0.1mile";
                case CSAFE_unit.unit_3_0_01_Mile: return "0.01 mile";
                case CSAFE_unit.unit_4_0_001_Mile: return "0.001 mile";
                case CSAFE_unit.unit_5_Ft: return "ft";
                case CSAFE_unit.unit_6_Inch: return "inch";
                case CSAFE_unit.unit_7_Lbs: return "lbs.";
                case CSAFE_unit.unit_8_0_1_Lbs: return "0.1 lbs.";
                case CSAFE_unit.unit_9_Undefined: return "undefined";
                case CSAFE_unit.unit_10_10_Ft: return "10 ft";
                case CSAFE_unit.unit_11_Undefined: return "undefined";
                case CSAFE_unit.unit_12_Undefined: return "undefined";
                case CSAFE_unit.unit_13_Undefined: return "undefined";
                case CSAFE_unit.unit_14_Undefined: return "undefined";
                case CSAFE_unit.unit_15_Undefined: return "undefined";
                case CSAFE_unit.unit_16_Mile_Hour: return "mile/hour";
                case CSAFE_unit.unit_17_0_1_Mile_Hour: return "0.1 mile/hour";
                case CSAFE_unit.unit_18_0_01_Mile_Hour: return "0.01 mile/hour";
                case CSAFE_unit.unit_19_Ft_Minute: return "ft/minute";
                case CSAFE_unit.unit_20_Undefined: return "undefined";
                case CSAFE_unit.unit_21_Undefined: return "undefined";
                case CSAFE_unit.unit_22_Undefined: return "undefined";
                case CSAFE_unit.unit_23_Undefined: return "undefined";
                case CSAFE_unit.unit_24_Undefined: return "undefined";
                case CSAFE_unit.unit_25_Undefined: return "undefined";
                case CSAFE_unit.unit_26_Undefined: return "undefined";
                case CSAFE_unit.unit_27_Undefined: return "undefined";
                case CSAFE_unit.unit_28_Undefined: return "undefined";
                case CSAFE_unit.unit_29_Undefined: return "undefined";
                case CSAFE_unit.unit_30_Undefined: return "undefined";
                case CSAFE_unit.unit_31_Undefined: return "undefined";
                case CSAFE_unit.unit_32_Undefined: return "undefined";
                case CSAFE_unit.unit_33_Km: return "Km";
                case CSAFE_unit.unit_34_0_1km: return "0.1km";
                case CSAFE_unit.unit_35_0_01km: return "0.01km";
                case CSAFE_unit.unit_36_Meter: return "Meter";
                case CSAFE_unit.unit_37_0_1_Meter: return "0.1 meter";
                case CSAFE_unit.unit_38_Cm: return "Cm";
                case CSAFE_unit.unit_39_Kg: return "Kg";
                case CSAFE_unit.unit_40_0_1_Kg: return "0.1 kg";
                case CSAFE_unit.unit_41_Undefined: return "undefined";
                case CSAFE_unit.unit_42_Undefined: return "undefined";
                case CSAFE_unit.unit_43_Undefined: return "undefined";
                case CSAFE_unit.unit_44_Undefined: return "undefined";
                case CSAFE_unit.unit_45_Undefined: return "undefined";
                case CSAFE_unit.unit_46_Undefined: return "undefined";
                case CSAFE_unit.unit_47_Undefined: return "undefined";
                case CSAFE_unit.unit_48_Km_Hour: return "Km/hour";
                case CSAFE_unit.unit_49_0_1Km_Hour: return "0.1Km/hour";
                case CSAFE_unit.unit_50_0_01_Km_Hour: return "0.01 Km/hour";
                case CSAFE_unit.unit_51_Meter_Minute: return "Meter/minute";
                case CSAFE_unit.unit_52_Undefined: return "undefined";
                case CSAFE_unit.unit_53_Undefined: return "undefined";
                case CSAFE_unit.unit_54_Undefined: return "undefined";
                case CSAFE_unit.unit_55_Minutes_Mile: return "Minutes/mile";
                case CSAFE_unit.unit_56_Minutes_Km: return "Minutes/km";
                case CSAFE_unit.unit_57_Seconds_Km: return "Seconds/km";
                case CSAFE_unit.unit_58_Seconds_Mile: return "Seconds/mile";
                case CSAFE_unit.unit_59_Undefined: return "undefined";
                case CSAFE_unit.unit_60_Undefined: return "undefined";
                case CSAFE_unit.unit_61_Undefined: return "undefined";
                case CSAFE_unit.unit_62_Undefined: return "undefined";
                case CSAFE_unit.unit_63_Undefined: return "undefined";
                case CSAFE_unit.unit_64_Undefined: return "undefined";
                case CSAFE_unit.unit_65_Floors: return "floors";
                case CSAFE_unit.unit_66_0_1_Floors: return "0.1 floors";
                case CSAFE_unit.unit_67_Steps: return "steps";
                case CSAFE_unit.unit_68_Revolutions: return "revolutions";
                case CSAFE_unit.unit_69_Strides: return "strides";
                case CSAFE_unit.unit_70_Strokes: return "strokes";
                case CSAFE_unit.unit_71_Beats: return "beats";
                case CSAFE_unit.unit_72_Calories: return "calories";
                case CSAFE_unit.unit_73_Kp: return "Kp";
                case CSAFE_unit.unit_74_Pct_Grade: return "% grade";
                case CSAFE_unit.unit_75_0_01_Pct_Grade: return "0.01 % grade";
                case CSAFE_unit.unit_76_0_1_Pct_Grade: return "0.1 % grade";
                case CSAFE_unit.unit_77_Undefined: return "undefined";
                case CSAFE_unit.unit_78_Undefined: return "undefined";
                case CSAFE_unit.unit_79_0_1_Floors_Minute: return "0.1 floors/minute";
                case CSAFE_unit.unit_80_Floors_Minute: return "floors/minute";
                case CSAFE_unit.unit_81_Steps_Minute: return "steps/minute";
                case CSAFE_unit.unit_82_Revs_Minute: return "revs/minute";
                case CSAFE_unit.unit_83_Strides_Minute: return "strides/minute";
                case CSAFE_unit.unit_84_Stokes_Minute: return "stokes/minute";
                case CSAFE_unit.unit_85_Beats_Minute: return "beats/minute";
                case CSAFE_unit.unit_86_Calories_Minute: return "calories/minute";
                case CSAFE_unit.unit_87_Calories_Hour: return "calories/hour";
                case CSAFE_unit.unit_88_Watts: return "Watts";
                case CSAFE_unit.unit_89_Kpm: return "Kpm";
                case CSAFE_unit.unit_90_Inch_Lb: return "Inch-Lb";
                case CSAFE_unit.unit_91_Foot_Lb: return "Foot-Lb";
                case CSAFE_unit.unit_92_Newton_Meters: return "Newton-Meters";
                case CSAFE_unit.unit_93_Undefined: return "undefined";
                case CSAFE_unit.unit_94_Undefined: return "undefined";
                case CSAFE_unit.unit_95_Undefined: return "undefined";
                case CSAFE_unit.unit_96_Undefined: return "undefined";
                case CSAFE_unit.unit_97_Amperes: return "Amperes";
                case CSAFE_unit.unit_98_0_001_Amperes: return "0.001 Amperes";
                case CSAFE_unit.unit_99_Volts: return "Volts";
                case CSAFE_unit.unit_100_0_001_Volts: return "0.001 Volts";
                case CSAFE_unit.unit_101_Undefined: return "undefined";
                case CSAFE_unit.unit_102_Undefined: return "undefined";
                case CSAFE_unit.unit_103_Undefined: return "undefined";
                case CSAFE_unit.unit_104_Undefined: return "undefined";
                case CSAFE_unit.unit_105_Undefined: return "undefined";
                case CSAFE_unit.unit_106_Undefined: return "undefined";
                case CSAFE_unit.unit_107_Undefined: return "undefined";
                case CSAFE_unit.unit_108_Undefined: return "undefined";
                case CSAFE_unit.unit_109_Undefined: return "undefined";
                case CSAFE_unit.unit_110_Undefined: return "undefined";
                case CSAFE_unit.unit_111_Undefined: return "undefined";
                case CSAFE_unit.unit_112_Undefined: return "undefined";
                case CSAFE_unit.unit_113_Undefined: return "undefined";
                case CSAFE_unit.unit_114_Undefined: return "undefined";
                case CSAFE_unit.unit_115_Undefined: return "undefined";
                case CSAFE_unit.unit_116_Undefined: return "undefined";
                case CSAFE_unit.unit_117_Undefined: return "undefined";
                case CSAFE_unit.unit_118_Undefined: return "undefined";
                case CSAFE_unit.unit_119_Undefined: return "undefined";
                case CSAFE_unit.unit_120_Undefined: return "undefined";
                case CSAFE_unit.unit_121_Undefined: return "undefined";
                case CSAFE_unit.unit_122_Undefined: return "undefined";
                case CSAFE_unit.unit_123_Undefined: return "undefined";
                case CSAFE_unit.unit_124_Undefined: return "undefined";
                case CSAFE_unit.unit_125_Undefined: return "undefined";
                case CSAFE_unit.unit_126_Undefined: return "undefined";
                case CSAFE_unit.unit_127_Undefined: return "undefined";
                case CSAFE_unit.unit_128_Undefined: return "undefined";
                default: return "unhandled value";
            }
        }
    }
}