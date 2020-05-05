using System;
using System.Collections.Generic;
using System.Text;

namespace Raw2Jpeg.TiffStructure
{
    public struct TiffTag
    {
        public TiffTag(ref byte[] Content, uint adressOffset, bool ISBigEndian)
        {
            byte[] bTagID;
            byte[] bDataType;
            byte[] bDataCount;
            byte[] bDataOffset;

            if (ISBigEndian)
            {
                bTagID = new byte[] { Content[adressOffset+1], Content[adressOffset] };
                bDataType = new byte[] { Content[adressOffset+3], Content[adressOffset+2] };
                bDataCount = new byte[] { Content[adressOffset+7], Content[adressOffset+6], Content[adressOffset+5], Content[adressOffset+4] };
                bDataOffset = new byte[] { Content[adressOffset+11], Content[adressOffset+10], Content[adressOffset+9], Content[adressOffset+8] };

            }
            else
            {
                bTagID = new byte[] { Content[adressOffset], Content[adressOffset+1] };
                bDataType = new byte[] { Content[adressOffset+2], Content[adressOffset+3] };
                bDataCount = new byte[] { Content[adressOffset+4], Content[adressOffset+5], Content[adressOffset+6], Content[adressOffset+7] };
                bDataOffset = new byte[] { Content[adressOffset+8], Content[adressOffset+9], Content[adressOffset+10], Content[adressOffset+11] };
            }
            TagID = BitConverter.ToUInt16(bTagID, 0);
            if (TiffTagIDValue.TagValue.ContainsKey(TagID))
                TagName = TiffTagIDValue.TagValue[TagID];
            else
                TagName = "";
            DataType = BitConverter.ToUInt16(bDataType, 0);
            DataCount = BitConverter.ToUInt32(bDataCount, 0);
            DataOffset = BitConverter.ToUInt32(bDataOffset, 0);
            TagValue = null;
            TagValue = TiffType.getValue(this,ref Content, ISBigEndian);
        }

        public ushort TagID { get; private set; }
        public string TagName { get; private set; }
        public ushort DataType { get; private set; }
        public uint DataCount { get; private set; }
        public uint DataOffset { get; private set; }

        public object TagValue {
            get;
            private set;
        }
    }
}
