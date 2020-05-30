using Raw2Jpeg.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Raw2Jpeg.RawStructure
{
    public class Raw : IRaw
    {
        RawBase rb;
        byte[] bitmap;
        Dictionary<string, string> dicMetadata;
        public Raw(byte[] Content)
        {
            rb = new RawBase(ref Content);

        }

        public byte[] Bitmap
        {
            get
            {
                if (bitmap == null)
                    bitmap = rb.Bitmap;
                return bitmap;

            }
        }
        public Dictionary<string, string> MetaData
        {
            get
            {
                if (dicMetadata == null)
                    dicMetadata = rb.MetaData;
                return dicMetadata;
            }
        }

        public ushort Orientation
        {
            get
            {
                ushort rValue = 0;
                var o = (from m in dicMetadata where m.Key == "Orientation" select m.Value).FirstOrDefault();
                if (o != null)
                    ushort.TryParse(o, out rValue);
                return rValue;
            }   
        }

        public uint Width
        {
            get
            {
                uint rValue = 0;
                var o = (from m in dicMetadata where m.Key == "Width" select m.Value).FirstOrDefault();
                if (o != null)
                    uint.TryParse(o, out rValue);
                return rValue;

            }
        }
        public uint Height
        {
            get {
                uint rValue = 0;
                var o = (from m in dicMetadata where m.Key == "Height" select m.Value).FirstOrDefault();
                if (o != null)
                    uint.TryParse(o, out rValue);
                return rValue;
            }
        }
    }
}
