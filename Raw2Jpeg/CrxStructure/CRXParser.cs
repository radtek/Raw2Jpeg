using Raw2Jpeg.TiffStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Raw2Jpeg.CrxStructure
{
    static class CRXParser
    {
        public static PRVW ParsePRVW(byte[] content, uint offset)
        {
            PRVW retValue = new PRVW(content, offset);
            
            return retValue;
        }


    }
}
