using PhotoNet.Common;

namespace RawNet
{
    public class FileIOException : RawDecoderException
    {
        public FileIOException(string error) : base(error)
        {

        }
    }
}
