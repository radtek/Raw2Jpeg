using System;
using System.Collections.Generic;
using System.Text;

namespace Raw2Jpeg.RawStructure
{
    interface IMetaNode
    {

        IMetaNode Childrens { get; internal set; }
        Object Value { get; internal set; }
    }
}
