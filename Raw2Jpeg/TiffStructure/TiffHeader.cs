
using Raw2Jpeg.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raw2Jpeg.TiffStructure
{
    internal class TiffHeader
    {
        private byte[] _content;
        private RawType _rawType;
        private TiffIFD[] tiffIFDs;
        public TiffHeader(ref byte[] header)
        {
            _content = header;
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
            tiffIFDs = FillTifID(AdressIFD);
        }

        public string Endian { get; set; }
        public int TIFF { get; set; }
        public uint AdressIFD { get; set; }
        public bool ISBigEndian { get; set; }

        public TiffIFD[] IFDs { get { return tiffIFDs; } }

        private TiffIFD[] FillTifID(uint adressIFD)
        {
            bool moreIFD = true;
            List<TiffIFD> tiffIFDs = new List<TiffIFD>();
            do
            {
                TiffIFD ifd = FillIFD(adressIFD);
                tiffIFDs.Add(ifd);
                if (ifd.NextIFDOffset == 0)
                    moreIFD = false;
                else
                {
                    adressIFD = ifd.NextIFDOffset;
                }
            } while (moreIFD);
            return tiffIFDs.ToArray();
        }

        private TiffIFD FillIFD(uint adressIFD)
        {
            ushort tagCount;
            if (ISBigEndian)
                tagCount = BitConverter.ToUInt16(new byte[] { _content[adressIFD + 1], _content[adressIFD] }, 0);
            else
                tagCount = BitConverter.ToUInt16(new byte[] { _content[adressIFD], _content[adressIFD + 1] }, 0);

            var ifd = new TiffIFD(ref _content, adressIFD, ISBigEndian);
            //var isDNG = (from tt in ifd.tiffTags where tt.TagID == 50706 select tt).FirstOrDefault().TagID != 50706;
            var makeValue = (from tt in ifd.tiffTags where tt.TagID == 271 select tt.TagValue).FirstOrDefault();
            if (makeValue != null)
                switch (makeValue.ToString())
                {
                    case string a when a.ToUpper().Contains("NIKON"):
                        _rawType = RawType.NEF;
                        break;
                    case string a when a.ToUpper().Contains("SONY"):
                        _rawType = RawType.ARW;
                        break;
                    case string a when a.ToUpper().Contains("CANON"):
                        _rawType = RawType.CR2;
                        break;
                    case string a when a.ToUpper().Contains("HASSELBLAD"):
                        _rawType = RawType.Hasselblad;
                        break;

                }
            //if (isDNG)
            //    _rawType = RawType.DNG;
            if (ifd.HasSubIFD)
            {
                var tIFD = (from t in ifd.tiffTags where t.TagID == 330 select t).FirstOrDefault();
                var Model = (from t in ifd.tiffTags where t.TagID == 272 select t.TagValue).FirstOrDefault().ToString();
                List<TiffIFD> lstsubID = new List<TiffIFD>();
                for (int miind = 0; miind < tIFD.DataCount; miind++)
                {
                    uint SubifdOffset;
                    if (ISBigEndian)
                        SubifdOffset = BitConverter.ToUInt32(new byte[] { _content[(4 * miind) + tIFD.DataOffset + 3], _content[(4 * miind) + tIFD.DataOffset + 2], _content[(4 * miind) + tIFD.DataOffset + 1], _content[(4 * miind) + tIFD.DataOffset] }, 0);
                    else
                        SubifdOffset = BitConverter.ToUInt32(new byte[] { _content[(4 * miind) + tIFD.DataOffset], _content[(4 * miind) + tIFD.DataOffset + 1], _content[(4 * miind) + tIFD.DataOffset + 2], _content[(4 * miind) + tIFD.DataOffset + 3] }, 0);

                    if (_rawType == RawType.ARW || Model.ToUpper().Contains("HASSELBLAD X1D\0"))
                        SubifdOffset = tIFD.DataOffset;
                    lstsubID.Add(FillIFD(SubifdOffset));
                }
                ifd.SubIFDS = lstsubID.ToArray();
            }
            return ifd;
        }


    }
}
