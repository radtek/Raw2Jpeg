using Raw2Jpeg.TiffStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Raw2Jpeg.Raw
{
    public interface IRaw
    {
        byte[] Bitmap { get; }
        TiffTag[] TiffTags { get; } 
    }
}
