using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;


namespace PhotoNet.Common
{
    public class JPEGThumbnail : Thumbnail
    {
        public byte[] data;

        public JPEGThumbnail(byte[] v)
        {
            data = v;
        }

        public Image GetBitmap()
        {
            if (data == null) return null;
            MemoryStream ms = new MemoryStream(data);
            var bitmap=Image.FromStream(ms);

            return bitmap;
        }
    }
}
