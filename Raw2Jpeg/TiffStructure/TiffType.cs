using System;
using System.Collections.Generic;
using System.Text;

namespace Raw2Jpeg.TiffStructure
{
    public static class TiffType
    {
        public static object getValue(TiffTag tag, ref byte[] binput, bool isBigEndian)
        {
            object objReturn = null;
            switch (tag.DataType)
            {
                case 1:
                    objReturn = getUByte(tag, binput);
                    break;
                case 2:
                    objReturn = getASCII(tag, binput, isBigEndian);
                    break;
                case 3:
                    objReturn = getUShort(tag, binput, isBigEndian);
                    break;
                case 4:
                    objReturn = getUInt(tag, binput, isBigEndian);
                    break;
                case 5:
                    objReturn = getULong(tag, binput, isBigEndian);
                    break;
                case 6:
                    objReturn = getByte(tag, binput);
                    break;
                case 7:
                    objReturn = getUndefined(tag, binput, isBigEndian);
                    break;
                case 8:
                    objReturn = getShort(tag, binput, isBigEndian);
                    break;
                case 9:
                    objReturn = getInt(tag, binput, isBigEndian);
                    break;
                case 10:
                    objReturn = getLong(tag, binput, isBigEndian);
                    break;
                case 11:
                    objReturn = getFloat(tag, binput, isBigEndian);
                    break;
                case 12:
                    objReturn = getDouble(tag, binput, isBigEndian);
                    break;
            }
            return objReturn;
        }

        public static double getDouble(TiffTag tag, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToDouble(new byte[] { binput[tag.DataOffset + 7], binput[tag.DataOffset + 6], binput[tag.DataOffset + 5], binput[tag.DataOffset + 4], binput[tag.DataOffset + 3], binput[tag.DataOffset + 2], binput[tag.DataOffset + 1], binput[tag.DataOffset] });
            return BitConverter.ToDouble(new byte[] { binput[tag.DataOffset], binput[tag.DataOffset + 1], binput[tag.DataOffset + 2], binput[tag.DataOffset + 3], binput[tag.DataOffset + 4], binput[tag.DataOffset + 5], binput[tag.DataOffset + 6], binput[tag.DataOffset + 7] });
        }

        public static float getFloat(TiffTag tag, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToSingle(new byte[] { binput[tag.DataOffset + 3], binput[tag.DataOffset + 2], binput[tag.DataOffset + 1], binput[tag.DataOffset] });
            return BitConverter.ToSingle(new byte[] { binput[tag.DataOffset], binput[tag.DataOffset + 1], binput[tag.DataOffset + 2], binput[tag.DataOffset + 3] });
        }

        public static long getLong(TiffTag tag, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToInt64(new byte[] { binput[tag.DataOffset + 7], binput[tag.DataOffset + 6], binput[tag.DataOffset + 5], binput[tag.DataOffset + 4], binput[tag.DataOffset + 3], binput[tag.DataOffset + 2], binput[tag.DataOffset + 1], binput[tag.DataOffset] });
            return BitConverter.ToInt64(new byte[] { binput[tag.DataOffset], binput[tag.DataOffset + 1], binput[tag.DataOffset + 2], binput[tag.DataOffset + 3], binput[tag.DataOffset + 4], binput[tag.DataOffset + 5], binput[tag.DataOffset + 6], binput[tag.DataOffset + 7] });
        }

        public static int getInt(TiffTag tag, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToInt32(new byte[] { binput[tag.DataOffset + 3], binput[tag.DataOffset + 2], binput[tag.DataOffset + 1], binput[tag.DataOffset] });
            return BitConverter.ToInt32(new byte[] { binput[tag.DataOffset], binput[tag.DataOffset + 1], binput[tag.DataOffset + 2], binput[tag.DataOffset + 3] });
        }

        public static short getShort(TiffTag tag, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToInt16(new byte[] { binput[tag.DataOffset + 1], binput[tag.DataOffset] });
            return BitConverter.ToInt16(new byte[] { binput[tag.DataOffset], binput[tag.DataOffset + 1] });
        }

        public static object getUndefined(TiffTag tag, byte[] binput, bool isBigEndian)
        {
            throw new NotImplementedException();
        }

        public static sbyte getByte(TiffTag tag, byte[] binput)
        {
            return Convert.ToSByte(binput[tag.DataOffset]);
        }

        public static ulong getULong(TiffTag tag, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToUInt64(new byte[] { binput[tag.DataOffset + 7], binput[tag.DataOffset + 6], binput[tag.DataOffset + 5], binput[tag.DataOffset + 4], binput[tag.DataOffset + 3], binput[tag.DataOffset + 2], binput[tag.DataOffset + 1], binput[tag.DataOffset] });
            return BitConverter.ToUInt64(new byte[] { binput[tag.DataOffset], binput[tag.DataOffset + 1], binput[tag.DataOffset + 2], binput[tag.DataOffset + 3], binput[tag.DataOffset + 4], binput[tag.DataOffset + 5], binput[tag.DataOffset + 6], binput[tag.DataOffset + 7] });
        }

        public static uint getUInt(TiffTag tag, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToUInt32(new byte[] { binput[tag.DataOffset + 3], binput[tag.DataOffset + 2], binput[tag.DataOffset + 1], binput[tag.DataOffset] });
            return BitConverter.ToUInt32(new byte[] { binput[tag.DataOffset], binput[tag.DataOffset + 1], binput[tag.DataOffset + 2], binput[tag.DataOffset + 3] });
        }

        public static ushort getUShort(TiffTag tag, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToUInt16(new byte[] { binput[tag.DataOffset + 1], binput[tag.DataOffset] });
            return BitConverter.ToUInt16(new byte[] { binput[tag.DataOffset], binput[tag.DataOffset + 1] });
        }

        public static string getASCII(TiffTag tag, byte[] binput, bool isBigEndian)
        {
            StringBuilder sb = new StringBuilder();
            for (int miind = 0; miind < tag.DataCount; miind++)
                sb.Append(BitConverter.ToChar(binput, miind + (int)tag.DataOffset));
            return sb.ToString();
        }

        public static byte getUByte(TiffTag tag, byte[] binput)
        {
            return Convert.ToByte(binput[tag.DataOffset]);
        }
    }
}
