using System;
using System.Collections.Generic;
using System.Text;

namespace Raw2Jpeg.TiffStructure
{
    internal struct TiffTag
    {
        public TiffTag(byte[] Tag, bool ISBigEndian)
        {
            byte[] bTagID;
            byte[] bDataType;
            byte[] bDataCount;
            byte[] bDataOffset;

            if (ISBigEndian)
            {
                bTagID = new byte[] { Tag[1], Tag[0] };
                bDataType = new byte[] { Tag[3], Tag[2] };
                bDataCount = new byte[] { Tag[7], Tag[6], Tag[5], Tag[4] };
                bDataOffset = new byte[] { Tag[11], Tag[10], Tag[9], Tag[8] };

            }
            else
            {
                bTagID = new byte[] { Tag[0], Tag[1] };
                bDataType = new byte[] { Tag[2], Tag[3] };
                bDataCount = new byte[] { Tag[4], Tag[5], Tag[6], Tag[7] };
                bDataOffset = new byte[] { Tag[8], Tag[9], Tag[10], Tag[11] };
            }
            TagID = BitConverter.ToUInt16(bTagID, 0);
            if (TiffTagIDValue.TagValue.ContainsKey(TagID))
                TagName = TiffTagIDValue.TagValue[TagID];
            else
                TagName = "";
            DataType = BitConverter.ToUInt16(bDataType, 0);
            DataCount = BitConverter.ToUInt32(bDataCount, 0);
            DataOffset = BitConverter.ToUInt32(bDataOffset, 0);

        }

        public ushort TagID { get; set; }
        public string TagName { get; set; }
        public ushort DataType { get; set; }

        public uint DataCount { get; set; }

        public uint DataOffset { get; set; }

    }
}
