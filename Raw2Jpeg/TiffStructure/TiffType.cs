using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Raw2Jpeg.TiffStructure
{
    internal static class TiffType
    {
        public static object getValue(TiffTag tag, ref byte[] binput, bool isBigEndian)
        {
            object objReturn = null;
            switch (tag.DataType)
            {
                case 1:
                    if (tag.DataCount * sizeof(byte) <= 4)
                        objReturn = (object)BitConverter.GetBytes(tag.DataOffset);
                    else
                        objReturn = getUByte(tag.DataOffset, binput);
                    break;
                case 2:
                    objReturn = GetString(tag.DataCount, tag.DataOffset, binput, isBigEndian);
                    break;
                case 3:
                    if (tag.DataCount*sizeof(ushort)<=4)
                    {
                        byte[] bs = BitConverter.GetBytes(tag.DataOffset);
                        if (tag.DataCount == 1)
                            objReturn = getUShort(0, bs, isBigEndian);
                        else
                        {
                            ushort[] us = new ushort[2];
                            us[0] = getUShort(0, bs, isBigEndian);
                            us[1] = getUShort(1, bs, isBigEndian);
                            objReturn = us;
                        }
                    }
                    else
                    {
                        List<ushort> lstUS = new List<ushort>();
                        for (uint miind = 0; miind < tag.DataCount; miind++)
                            lstUS.Add(getUShort(tag.DataOffset + miind * sizeof(ushort), binput, isBigEndian));
                        objReturn = lstUS.ToArray();
                    }
                    break;
                case 4:
                    if (tag.DataCount * sizeof(uint) <= 4)
                    {
                        byte[] bs = BitConverter.GetBytes(tag.DataOffset);
                        objReturn = getUInt(0,bs,isBigEndian);
                    }
                    else
                    {
                        List<uint> lstui = new List<uint>();
                        for (uint miind = 0; miind < tag.DataCount; miind++)
                            lstui.Add(getUInt(tag.DataOffset + miind * sizeof(uint), binput, isBigEndian));
                        objReturn = lstui.ToArray();
                    }
                    break;
                case 5:
                    {
                        List<float> lstul = new List<float>();
                        for (uint miind = 0; miind < tag.DataCount; miind++)
                            lstul.Add(getRational(tag.DataOffset + miind * sizeof(long), binput, isBigEndian));
                        objReturn = lstul.ToArray();
                    }
                    break;
                case 6:
                    if (tag.DataCount * sizeof(byte) <= 4)
                        objReturn = (object)BitConverter.GetBytes(tag.DataOffset);
                    else
                        objReturn = getByte(tag.DataOffset, binput);
                    break;
                case 7:
                    objReturn = getUndefined(tag.DataOffset, binput, isBigEndian);
                    break;
                case 8:
                    if (tag.DataCount * sizeof(short) <= 4)
                    {
                        byte[] bs = BitConverter.GetBytes(tag.DataOffset);
                        if (tag.DataCount == 1)
                            objReturn = getShort(0, bs, isBigEndian);
                        else
                        {
                            short[] us = new short[2];
                            us[0] = getShort(0, bs, isBigEndian);
                            us[1] = getShort(1, bs, isBigEndian);
                            objReturn = us;
                        }
                    }
                    else
                    {
                        List<short> lstUS = new List<short>();
                        for (uint miind = 0; miind < tag.DataCount; miind++)
                            lstUS.Add(getShort(tag.DataOffset + miind * sizeof(ushort), binput, isBigEndian));
                        objReturn = lstUS.ToArray();
                    }
                    break;
                case 9:
                    if (tag.DataCount * sizeof(int) <= 4)
                    {
                        byte[] bs = BitConverter.GetBytes(tag.DataOffset);
                        objReturn = getInt(0, bs, isBigEndian);
                    }
                    else
                    {
                        List<int> lstui = new List<int>();
                        for (uint miind = 0; miind < tag.DataCount; miind++)
                            lstui.Add(getInt(tag.DataOffset + miind * sizeof(int), binput, isBigEndian));
                        objReturn = lstui.ToArray();
                    }
                    break;
                case 10:
                    {
                        List<float> lstul = new List<float>();
                        for (uint miind = 0; miind < tag.DataCount; miind++)
                            lstul.Add(getRational(tag.DataOffset + miind * sizeof(long), binput, isBigEndian));
                        objReturn = lstul.ToArray();
                    }
                    break;
                case 11:
                    {
                        List<float> lstul = new List<float>();
                        for (uint miind = 0; miind < tag.DataCount; miind++)
                            lstul.Add(getFloat(tag.DataOffset + miind * sizeof(long), binput, isBigEndian));
                        objReturn = lstul.ToArray();
                    }
                    break;
                case 12:
                    List<double> lstdb = new List<double>();
                    for (uint miind = 0; miind < tag.DataCount; miind++)
                        lstdb.Add(getDouble(tag.DataOffset + miind * sizeof(double), binput, isBigEndian));
                    objReturn = lstdb.ToArray();
                    break;
            }
            return objReturn;
        }

        private static float getRational(uint dataOffset, byte[] binput, bool isBigEndian)
        {
            var numerator = getInt(dataOffset, binput, isBigEndian);
            var denominator = getInt(dataOffset+4, binput, isBigEndian);
            return numerator / denominator;
        }

        private static object GetString(uint dataCount, uint dataOffset, byte[] binput, bool isBigEndian)
        {
            ASCIIEncoding aSCII = new ASCIIEncoding();
            byte[] bascii = new byte[dataCount];
            Array.Copy(binput, dataOffset, bascii, 0, dataCount);
            return aSCII.GetString(bascii);
        }

        public static double getDouble(uint DataOffset, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToDouble(new byte[] { binput[DataOffset + 7], binput[DataOffset + 6], binput[DataOffset + 5], binput[DataOffset + 4], binput[DataOffset + 3], binput[DataOffset + 2], binput[DataOffset + 1], binput[DataOffset] },0);
            return BitConverter.ToDouble(new byte[] { binput[DataOffset], binput[DataOffset + 1], binput[DataOffset + 2], binput[DataOffset + 3], binput[DataOffset + 4], binput[DataOffset + 5], binput[DataOffset + 6], binput[DataOffset + 7] },0);
        }

        public static float getFloat(uint DataOffset, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToSingle(new byte[] { binput[DataOffset + 3], binput[DataOffset + 2], binput[DataOffset + 1], binput[DataOffset] },0);
            return BitConverter.ToSingle(new byte[] { binput[DataOffset], binput[DataOffset + 1], binput[DataOffset + 2], binput[DataOffset + 3] },0);
        }

        public static long getLong(uint DataOffset, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToInt64(new byte[] { binput[DataOffset + 7], binput[DataOffset + 6], binput[DataOffset + 5], binput[DataOffset + 4], binput[DataOffset + 3], binput[DataOffset + 2], binput[DataOffset + 1], binput[DataOffset] },0);
            return BitConverter.ToInt64(new byte[] { binput[DataOffset], binput[DataOffset + 1], binput[DataOffset + 2], binput[DataOffset + 3], binput[DataOffset + 4], binput[DataOffset + 5], binput[DataOffset + 6], binput[DataOffset + 7] },0);
        }

        public static int getInt(uint DataOffset, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToInt32(new byte[] { binput[DataOffset + 3], binput[DataOffset + 2], binput[DataOffset + 1], binput[DataOffset] },0);
            return BitConverter.ToInt32(new byte[] { binput[DataOffset], binput[DataOffset + 1], binput[DataOffset + 2], binput[DataOffset + 3] },0);
        }

        public static short getShort(uint DataOffset, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToInt16(new byte[] { binput[DataOffset + 1], binput[DataOffset] },0);
            return BitConverter.ToInt16(new byte[] { binput[DataOffset], binput[DataOffset + 1] },0);
        }

        public static object getUndefined(uint DataOffset, byte[] binput, bool isBigEndian)
        {
            throw new NotImplementedException();
        }

        public static sbyte getByte(uint DataOffset, byte[] binput)
        {
            return Convert.ToSByte(binput[DataOffset]);
        }

        public static ulong getULong(uint DataOffset, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToUInt64(new byte[] { binput[DataOffset + 7], binput[DataOffset + 6], binput[DataOffset + 5], binput[DataOffset + 4], binput[DataOffset + 3], binput[DataOffset + 2], binput[DataOffset + 1], binput[DataOffset] },0);
            return BitConverter.ToUInt64(new byte[] { binput[DataOffset], binput[DataOffset + 1], binput[DataOffset + 2], binput[DataOffset + 3], binput[DataOffset + 4], binput[DataOffset + 5], binput[DataOffset + 6], binput[DataOffset + 7] },0);
        }

        public static uint getUInt(uint DataOffset, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToUInt32(new byte[] { binput[DataOffset + 3], binput[DataOffset + 2], binput[DataOffset + 1], binput[DataOffset] },0);
            return BitConverter.ToUInt32(new byte[] { binput[DataOffset], binput[DataOffset + 1], binput[DataOffset + 2], binput[DataOffset + 3] },0);
        }

        public static ushort getUShort(uint DataOffset, byte[] binput, bool isBigEndian)
        {
            if (isBigEndian)
                return BitConverter.ToUInt16(new byte[] { binput[DataOffset + 1], binput[DataOffset] },0);
            return BitConverter.ToUInt16(new byte[] { binput[DataOffset], binput[DataOffset + 1] },0);
        }

        public static char getASCII(uint DataOffset, byte[] binput, bool isBigEndian)
        {
            return BitConverter.ToChar(binput,(int) DataOffset);
             
        }

        public static byte getUByte(uint DataOffset, byte[] binput)
        {
            return Convert.ToByte(binput[DataOffset]);
        }
    }
}
