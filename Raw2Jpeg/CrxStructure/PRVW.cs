using Raw2Jpeg.TiffStructure;
using System;
using System.Text;

namespace Raw2Jpeg.CrxStructure
{
    public class PRVW
    {
        public PRVW(byte[] content, uint offset)
        {
            Size = TiffType.getInt(offset, content, true); ;
            Name = Encoding.ASCII.GetString(content, (int)offset + 4, 4).ToString();

            JPGSize = TiffType.getInt(offset + 0x14, content, true);
            JPG = new byte[JPGSize];
            Array.Copy(content, offset + 0x18, JPG, 0, JPGSize);

        }
        public int Size { get; set; }
        public string Name { get; set; }
        public int JPGSize { get; set; }
        public byte[] JPG { get; set; }
    }
}