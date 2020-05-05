using System;
using System.Collections.Generic;
using System.Text;

namespace Raw2Jpeg.TiffStructure
{
    internal struct TiffHeader
    {
        public TiffHeader(byte[] header)
        {
            Endian = string.Format("{0}{1}", (char)header[1], (char)header[0]);
            ISBigEndian = Endian == "MM";
            byte[] bTiff;
            byte[] bAdressIFD;
            if (ISBigEndian)
            {
                bTiff = new byte[] { header[3], header[2] };
                bAdressIFD = new byte[] { header[7], header[6], header[5], header[4] };
            }
            else
            {
                bTiff = new byte[] { header[2], header[3] };
                bAdressIFD = new byte[] { header[4], header[5], header[6], header[7] };
            }
            TIFF = BitConverter.ToInt16(bTiff, 0);
            AdressIFD = BitConverter.ToUInt32(bAdressIFD, 0);
        }

        public string Endian { get; set; }
        public int TIFF { get; set; }
        public uint AdressIFD { get; set; }

        public bool ISBigEndian { get; set; }

    }
}
