using Raw2Jpeg.TiffStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Raw2Jpeg.CrxStructure
{
    public class CRXHeader 
    {
        bool isBigEndian = true;
        byte[] _content;
        public CRXHeader(ref byte[] Content)
        {
            this._content = Content;
            AdressIFD = 0;
            IFDs = FillIFD();
        }

        private CRXIFD[] FillIFD()
        {

        List<CRXIFD> lstIIFD = new List<CRXIFD>();
            do {
                CRXIFD ifd = new CRXIFD(this._content, AdressIFD);
                if (ifd.Size == 0)
                    break;
                AdressIFD = ifd.NextIFDOffset;
                lstIIFD.Add(ifd);

            } while (AdressIFD<_content.Length);
            return lstIIFD.ToArray();
        }

        public uint AdressIFD { get; set; }

        public CRXIFD[] IFDs { get; set; }
    }
}
