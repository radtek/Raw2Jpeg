using Raw2Jpeg;
using System;
using System.IO;
using System.Linq;

namespace TestLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = 1;
            var b = a << 1;
            var c = a >> 1;

            string[] files = Directory.GetFiles(@"C:\temp\Source");
            foreach (var fileName in files)
            {
                var pathName = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(fileName);
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] bInput = new byte[fs.Length];
                fs.Read(bInput, 0, (int)fs.Length);
                RawBase tiff = new RawBase(ref bInput);
                if(tiff.Bitmap!=default(byte[]))
                    File.WriteAllBytes(string.Format("c:\\temp\\Destination\\{0}.jpg", pathName), tiff.Bitmap);
            }

        }
    }
}
