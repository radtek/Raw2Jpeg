using Raw2Jpeg.TiffStructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Raw2Jpeg
{
    public abstract class TiffBase : IDisposable
    {
        byte[] _content;
        TiffIFD[] _tiffIFDs;
        TiffHeader _header;
        RawType _rawType;
        public TiffBase(ref byte[] Content)
        {
            _rawType = RawType.Tiff;
            this._content = Content;
            _header = new TiffHeader(ref Content);
            _tiffIFDs = FillTifID( _header.AdressIFD);
        }


        private TiffIFD[] FillTifID( uint adressIFD)
        {
            bool moreIFD = true;
            List<TiffIFD> tiffIFDs = new List<TiffIFD>();
            do
            {
                TiffIFD ifd = FillIFD( adressIFD);
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
            if (IsBigEndian)
                tagCount = BitConverter.ToUInt16(new byte[] { _content[adressIFD + 1], _content[adressIFD] },0);
            else
                tagCount = BitConverter.ToUInt16(new byte[] { _content[adressIFD], _content[adressIFD + 1] },0);


            int lengthIFD = tagCount * 12 + 6;
            byte[] bIFD = new byte[lengthIFD];
            var ifd = new TiffIFD(ref _content, adressIFD, IsBigEndian);
            if (ifd.HasSubIFD)
            {
                var tIFD = (from t in ifd.tiffTags where t.TagID == 330 select t).FirstOrDefault();
                List<TiffIFD> lstsubID = new List<TiffIFD>();
                for (int miind = 0; miind < tIFD.DataCount; miind++)
                {
                    uint SubifdOffset;
                    if (IsBigEndian)
                        SubifdOffset = BitConverter.ToUInt32(new byte[] { _content[(4 * miind) + tIFD.DataOffset + 3], _content[(4 * miind) + tIFD.DataOffset + 2], _content[(4 * miind) + tIFD.DataOffset + 1], _content[(4 * miind) + tIFD.DataOffset] }, 0);
                    else
                        SubifdOffset = BitConverter.ToUInt32(new byte[] { _content[(4 * miind) + tIFD.DataOffset], _content[(4 * miind) + tIFD.DataOffset + 1], _content[(4 * miind) + tIFD.DataOffset + 2], _content[(4 * miind) + tIFD.DataOffset + 3] }, 0);

                    if (_rawType == RawType.ARW)
                        SubifdOffset = tIFD.DataOffset;
                    lstsubID.Add(FillIFD(SubifdOffset));
                }
                ifd.SubIFDS = lstsubID.ToArray();
            }
            return ifd;
        }

        public bool IsBigEndian { get { return _header.ISBigEndian; } }

        public TiffIFD[] TiffIFDs { get { return _tiffIFDs; } }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
