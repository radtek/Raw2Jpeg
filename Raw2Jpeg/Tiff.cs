using System;
using System.Collections.Generic;
using System.Text;

namespace Raw2Jpeg
{
    public class Tiff:RawBase
    {
        public Tiff(ref byte[] content) : base(ref content)
        { }
    }
}
