
using Raw2Jpeg.CrxStructure;
using Raw2Jpeg.Helper;
using Raw2Jpeg.TiffStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Raw2Jpeg
{
    public class RawBase
    {
        byte[] _content;
        TiffHeader _header;
        RawType _rawType;
        public RawBase(ref byte[] Content)
        {
            if (IsCR3(Content))
            {
                _rawType = RawType.CR3;
                this._content = Content;
                var crxheader = new CRXHeader(ref Content);
            }
            else
            {
                _rawType = RawType.Tiff;
                this._content = Content;
                _header = new TiffHeader(ref Content);
                
            }
        }

        private bool IsCR3(byte[] content)
        {
            byte[] bHeader = new byte[4];
            Array.Copy(content, 4, bHeader, 0 ,4);
            var StrHeader= Encoding.ASCII.GetString(bHeader);
            return StrHeader == "ftyp";
        }
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
                    case RawType.DNG:
                        return GetDNGBitmap();
                    default:
                        return GetTiffBitmap();
                }

            }

        }

        private byte[] GetDNGBitmap()
        {
            throw new NotImplementedException();
        }

        private byte[] Get3FRbitmap()
        {

            TiffIFD tagSub;
            if ( _header.IFDs[0].SubIFDS.Length > 1)
                tagSub = _header.IFDs[0].SubIFDS[1];
            else
                tagSub = _header.IFDs[0];
            
            var tIImageStartOffset = (from t in tagSub.tiffTags where t.TagID == 273 select t).FirstOrDefault().DataOffset;
            var tIImageLengthOffset = (from t in tagSub.tiffTags where t.TagID == 279 select t).FirstOrDefault().DataOffset;
            return CreateBitmap(tIImageStartOffset, tIImageLengthOffset, tagSub);
        }

        private byte[] GetTiffBitmap()
        {
            TiffIFD tagSub = _header.IFDs[0];

            var tIImageStartOffset = (from t in tagSub.tiffTags where t.TagID == 273 select t).FirstOrDefault().DataOffset;
            var tIImageLengthOffset = (from t in tagSub.tiffTags where t.TagID == 279 select t).FirstOrDefault().DataOffset;
            return CreateBitmap(tIImageStartOffset, tIImageLengthOffset, tagSub);
            
        }

        private byte[] GetNEFBitmap()
        {
            TiffIFD tagSub = _header.IFDs[0].SubIFDS[0];
            var tIImageStartOffset = (from t in tagSub.tiffTags where t.TagID == 513 select t).FirstOrDefault().DataOffset;
            var tIImageLengthOffset = (from t in tagSub.tiffTags where t.TagID == 514 select t).FirstOrDefault().DataOffset;
            return CreateBitmap(tIImageStartOffset, tIImageLengthOffset, tagSub);
        }

        private byte[] CreateBitmap(uint tIImageStartOffset, uint tIImageLengthOffset, TiffIFD tiffIFD)
        {
            byte[] Img = new byte[tIImageLengthOffset];
            Array.Copy(_content, tIImageStartOffset, Img, 0, tIImageLengthOffset);
            if (tiffIFD.Compression == 1 && tiffIFD.BitsPerSample[0] == 8 && tiffIFD.BitsPerSample[1] == 8 && tiffIFD.BitsPerSample[2] == 8)
            {
                return ImageHelper.ConvertFromUncompressed((int)tiffIFD.Width, (int)tiffIFD.Height, ref Img);
            }
            if (tiffIFD.Compression == 1 && tiffIFD.BitsPerSample.Length == 1 && tiffIFD.BitsPerSample[0] == 16)
            {
                //byte[] bs = ImageHelper.ConvertFrom16bits565((int)tiffIFD.Width, (int)tiffIFD.Height, ref Img);
                byte[] bs= ImageHelper.ConvertFromCFA((int)tiffIFD.Width, (int)tiffIFD.Height, ref Img);
                //bs = ImageHelper.BayerDemosaic24((int)tiffIFD.Width, (int)tiffIFD.Height, ref bs);
                //bs = ImageHelper.BayerDemosaic24((int)tiffIFD.Width, (int)tiffIFD.Height, ref bs);
                //byte[] bs = ImageHelper.ConvertFromCFA((int)tiffIFD.Width, (int)tiffIFD.Height, ref Img);
                ////return bs;
                //bs = ImageHelper.BayerDemosaic24((int)tiffIFD.Width, (int)tiffIFD.Height, ref bs);

                return ImageHelper.byteArrayToBMP((int)tiffIFD.Width, (int)tiffIFD.Height, ref bs);
            }
            if (tiffIFD.Compression ==1 && tiffIFD.BitsPerSample.Length == 1 && tiffIFD.BitsPerSample[0] == 12)
            {
                return ImageHelper.byteArrayToBMP((int)tiffIFD.Width, (int)tiffIFD.Height, ref Img);
            }

            return Img;
        }

        private byte[] GetCR2Bitmap()
        {

            TiffIFD tagSub = _header.IFDs[0];
            var tIImageStartOffset = (from t in tagSub.tiffTags where t.TagID == 273 select t).FirstOrDefault().DataOffset;
            var tIImageLengthOffset = (from t in tagSub.tiffTags where t.TagID == 279 select t).FirstOrDefault().DataOffset;
            return CreateBitmap(tIImageStartOffset, tIImageLengthOffset, tagSub);
        }

        private byte[] GetArwBitmap()
        {
            TiffIFD tagSub = _header.IFDs[0];
            var tIImageStartOffset = (from t in tagSub.tiffTags where t.TagID == 513 select t).FirstOrDefault().DataOffset;
            var tIImageLengthOffset = (from t in tagSub.tiffTags where t.TagID == 514 select t).FirstOrDefault().DataOffset;
            return CreateBitmap(tIImageStartOffset, tIImageLengthOffset, tagSub);
        }


    }
}
