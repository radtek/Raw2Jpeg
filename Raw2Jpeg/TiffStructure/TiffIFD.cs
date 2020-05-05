using System;
using System.Collections.Generic;
using System.Text;

namespace Raw2Jpeg.TiffStructure
{
    internal struct TiffIFD
    {
        public TiffIFD(byte[] bytes, bool ISBigEndian)
        {
            if (ISBigEndian)
            {
                tagCount = BitConverter.ToUInt16(new byte[] { bytes[1], bytes[0] });
                NextIFDOffset = BitConverter.ToUInt32(new byte[] { bytes[bytes.Length - 1], bytes[bytes.Length - 2], bytes[bytes.Length - 3], bytes[bytes.Length - 4] });
            }
            else
            {
                tagCount = BitConverter.ToUInt16(new byte[] { bytes[0], bytes[1] });
                NextIFDOffset = BitConverter.ToUInt32(new byte[] { bytes[bytes.Length - 4], bytes[bytes.Length - 3], bytes[bytes.Length - 2], bytes[bytes.Length - 1] });
            }
            tiffTags = new TiffTag[tagCount];


            for (int miind = 0; miind < tagCount; miind++)
            {
                byte[] bArray = new byte[12];
                Array.Copy(bytes, 2 + miind * 12, bArray, 0, 12);


                tiffTags[miind] = new TiffTag(bArray, ISBigEndian);

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
    }
}
}
