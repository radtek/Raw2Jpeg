using Raw2Jpeg.Raw;
using Raw2Jpeg.TiffStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography;

namespace Raw2Jpeg
{
    public class RawBase
    {
        byte[] _content;
        TiffIFD[] _tiffIFDs;
        TiffHeader _header;
        IRaw raw;
        RawType _rawType;
        public RawBase(ref byte[] Content)
        {
            _rawType = RawType.Tiff;
            this._content = Content;
            _header = new TiffHeader(ref Content);
            _tiffIFDs = FillTifID(_header.AdressIFD);
        }


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
            if (IsBigEndian)
                tagCount = BitConverter.ToUInt16(new byte[] { _content[adressIFD + 1], _content[adressIFD] }, 0);
            else
                tagCount = BitConverter.ToUInt16(new byte[] { _content[adressIFD], _content[adressIFD + 1] }, 0);

            var ifd = new TiffIFD(ref _content, adressIFD, IsBigEndian);
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

        public byte[] Bitmap
        {
            get
            {
                switch (_rawType)
                {
                    case RawType.ARW:
                        return GetArwBitmap();
                    case RawType.CR2:
                        return GetCR2Bitmap();
                    case RawType.NEF:
                        return GetNEFBitmap();
                    case RawType.Hasselblad:
                        return Get3FRbitmap();
                    default:
                        return GetTiffBitmap();
                }

            }

        }

        private byte[] Get3FRbitmap()
        {
            TiffTag[] tagSub = _tiffIFDs[0].tiffTags;
            var tIImageStartOffset = (from t in tagSub where t.TagID == 273 select t).FirstOrDefault().DataOffset;
            var tIImageLengthOffset = (from t in tagSub where t.TagID == 279 select t).FirstOrDefault().DataOffset;
            return CreateBitmap(tIImageStartOffset, tIImageLengthOffset);
        }

        private byte[] GetTiffBitmap()
        {
            return default(byte[]);
        }

        private byte[] GetNEFBitmap()
        {
            TiffTag[] tagSub = _tiffIFDs[0].SubIFDS[0].tiffTags;
            var tIImageStartOffset = (from t in tagSub where t.TagID == 513 select t).FirstOrDefault().DataOffset;
            var tIImageLengthOffset = (from t in tagSub where t.TagID == 514 select t).FirstOrDefault().DataOffset;
            return CreateBitmap(tIImageStartOffset, tIImageLengthOffset);
        }

        private byte[] CreateBitmap(uint tIImageStartOffset, uint tIImageLengthOffset)
        {
            byte[] Img = new byte[tIImageLengthOffset];
            Array.Copy(_content, tIImageStartOffset, Img, 0, tIImageLengthOffset);
            return Img;
        }

        private byte[] GetCR2Bitmap()
        {

            TiffTag[] tagSub = _tiffIFDs[0].tiffTags;
            var tIImageStartOffset = (from t in tagSub where t.TagID == 273 select t).FirstOrDefault().DataOffset;
            var tIImageLengthOffset = (from t in tagSub where t.TagID == 279 select t).FirstOrDefault().DataOffset;
            return CreateBitmap(tIImageStartOffset, tIImageLengthOffset);
        }

        private byte[] GetArwBitmap()
        {
            TiffTag[] tagSub = _tiffIFDs[0].tiffTags;
            var tIImageStartOffset = (from t in tagSub where t.TagID == 513 select t).FirstOrDefault().DataOffset;
            var tIImageLengthOffset = (from t in tagSub where t.TagID == 514 select t).FirstOrDefault().DataOffset;
            return CreateBitmap(tIImageStartOffset, tIImageLengthOffset);
        }


    }
}
