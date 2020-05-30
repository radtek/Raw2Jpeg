using Raw2Jpeg;
using Raw2Jpeg.Helper;
using Raw2Jpeg.RawStructure;
using System;
using System.Collections.Generic;
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
                Raw tiff = new Raw( bInput);
                var bmp = tiff.Bitmap;
                var md= tiff.MetaData;
                if (bmp != default(byte[]))
                {
                    byte[] bOut;
                    ImageHelper.AutoOrientation(tiff.Orientation, ref bmp, out bOut);
                    File.WriteAllBytes(string.Format("c:\\temp\\Destination\\{0}.jpg", pathName), bOut);
                }
                if (md != null)
                    File.WriteAllLines(string.Format("c:\\temp\\Destination\\{0}.txt", pathName), md.Select(x => "[" + x.Key + "]:\t" + x.Value).ToArray());
            }

        }
    }
}
