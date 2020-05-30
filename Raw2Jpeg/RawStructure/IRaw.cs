using System;
using System.Collections.Generic;
using System.Text;

namespace Raw2Jpeg.RawStructure
{
    interface IRaw
    {
        byte[] Bitmap { get; }
        Dictionary<string, string> MetaData { get; }
    }
}
