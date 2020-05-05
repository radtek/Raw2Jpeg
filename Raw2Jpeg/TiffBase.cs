using System;

namespace Raw2Jpeg
{
    public abstract class TiffBase : IDisposable
    {
        byte[] _content;
        public TiffBase(ref byte[] Content)
        {
            this._content = Content;
        }



        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
