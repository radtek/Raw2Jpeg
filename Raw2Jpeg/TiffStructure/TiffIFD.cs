using System;
using System.ComponentModel.Design;
using System.Linq;

namespace Raw2Jpeg.TiffStructure
{
    public struct TiffIFD
    {
        public TiffIFD(ref byte[] content, uint adressOffset, bool ISBigEndian)
        {
            uint ifdEnd;
            if (ISBigEndian)
            {
                tagCount = BitConverter.ToUInt16(new byte[] { content[adressOffset + 1], content[adressOffset] }, 0);
                ifdEnd = adressOffset + (uint)(tagCount * 12 + 6);
                NextIFDOffset = BitConverter.ToUInt32(new byte[] { content[ifdEnd - 1], content[ifdEnd - 2], content[ifdEnd - 3], content[ifdEnd - 4] }, 0);
            }
            else
            {
                tagCount = BitConverter.ToUInt16(new byte[] { content[adressOffset], content[adressOffset + 1] }, 0);
                ifdEnd = adressOffset + (uint)(tagCount * 12 + 6);
                NextIFDOffset = BitConverter.ToUInt32(new byte[] { content[ifdEnd - 4], content[ifdEnd - 3], content[ifdEnd - 2], content[ifdEnd - 1] }, 0);
            }
            tiffTags = new TiffTag[tagCount];


            for (int miind = 0; miind < tagCount; miind++)
            {
                byte[] bArray = new byte[12];
                tiffTags[miind] = new TiffTag(ref content, (uint)(adressOffset + 2 + miind * 12), ISBigEndian);

            }
            var tIFD = (from t in tiffTags where t.TagID == 330 select t).FirstOrDefault();
            HasSubIFD = tIFD.TagID == 330;
            SubIFDS = new TiffIFD[tIFD.DataCount];

        }
        public ushort tagCount { get; set; }
        public TiffTag[] tiffTags { get; set; }
        public uint NextIFDOffset { get; set; }

        public bool HasSubIFD { get; set; }

        public TiffIFD[] SubIFDS { get; set; }

        public ushort? Compression
        {
            get {
                var comp = (from t in tiffTags where t.TagID == 259 select t.TagValue).FirstOrDefault();
                if (comp == null)
                    return null;
                return (ushort)comp;
            }
        }

        public uint? Width
        {
            get {
                var width = (from t in tiffTags where t.TagID == 256 select t.TagValue).FirstOrDefault();
                if (width == null)
                    return null;
                if (width.GetType() == typeof(ushort))
                {
                    ushort w = (ushort)width;
                    return (uint)w;
                }
                return (uint)width;
            }
        }

        public uint? Height
        {
            get
            {
                var Height = (from t in tiffTags where t.TagID == 257 select t.TagValue).FirstOrDefault();
                if (Height == null)
                    return null;
                if (Height.GetType() == typeof(ushort))
                {
                    ushort h = (ushort)Height;
                    return (uint)h;
                }
                return (uint)Height;
            }
        }

        public ushort? Orientation
        {
            get {
                var Orientation = (from t in tiffTags where t.TagID == 274 select t.TagValue).FirstOrDefault();
                if (Orientation == null)
                    return null;
                return (ushort)Orientation;

            }
        }

        public short[] BitsPerSample {
            get {
                var bps = (from t in tiffTags where t.TagID == 258 select t.TagValue).FirstOrDefault();
                if (bps == null)
                    return default(short[]);
                if (bps.GetType() == typeof(ushort))
                {
                    var bs = (ushort)bps ;
                    return new short[] { (short)bs };
                }
                return (short[])bps;           
            }
        }

    }
}

