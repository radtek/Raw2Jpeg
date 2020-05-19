using Raw2Jpeg.TiffStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raw2Jpeg.CrxStructure
{
    public class CRXIFD
    {
        static bool isBigEndian = true;
        static Dictionary<string, int> dicCount = new Dictionary<string, int>();
        static List<string> tags = new List<string> { "ftyp", "moov", "uuid", "stsz", "co64", "PRVW", "CTBO", "THMB", "CNCV", "CDI1", "IAD1", "CMP1", "CRAW", "CNOP" };
        static List<string> cmt = new List<string> { "CMT1", "CMT2", "CMT3", "CMT4", "CMTA" };
        static List<string> innerParsing = new List<string> { "moov", "trak", "mdia", "minf", "dinf", "stbl" };
        static Dictionary<string, uint> innerOffsets = new Dictionary<string, uint>() { { "CRAW", 0x52 }, { "CCTP", 12 }, { "stsd", 8 }, { "dref", 8 }, { "CDI1", 4 } };

        static byte NameLen = 4;
        static byte SizeLen = 4;
        static byte UUIDLen = 16;





        private int size;
        private List<CRXIFD> lstCRXIFD = new List<CRXIFD>();
        private int no = 0;


        public CRXIFD(byte[] content, uint offset)
        {

            size = TiffType.getInt(offset, content, true);
            Name = Encoding.ASCII.GetString(content, (int)offset + SizeLen, NameLen).ToString();
            no = SizeLen + NameLen;
            if (size == 1)
            {
                size = TiffType.getInt((uint)(offset + no), content, true);
                no = SizeLen + NameLen + 8;
            }

            if (dicCount.Keys.Contains(Name))
            {
                dicCount[Name]++;
            }
            else
            {
                dicCount.Add(Name, 1);
            }

            if (tags.Contains(Name))
            {
                FillTags(Name,content, offset + no, size - no);

            }

            if (cmt.Contains(Name))
            {
                FillTiff(content, offset + no, size - no);
            }
            if (innerParsing.Contains(Name))
            {
                var adressOffsetParent= (uint)(offset + no);
                uint adressOffset=adressOffsetParent;
                do
                {
                    var crx = new CRXIFD(content, adressOffset);
                    adressOffset = crx.NextIFDOffset;
                    lstCRXIFD.Add(crx);
                } while (adressOffset < (adressOffsetParent + size));
            }
            if (innerOffsets.Keys.Contains(Name))
            {
                size= (int)(size - no - innerOffsets[Name]);
            }
            if (Name == "trak")
            {
                Name = "trak" + dicCount["trak"];
            }
            NextIFDOffset = (uint)(offset + size);
        }


        private void FillTiff(byte[] content, long offset, int size)
        {
            byte[] tiffContent = new byte[size];
            Array.Copy(content, offset, tiffContent, 0, size);

            TiffHeader header = new TiffHeader(ref tiffContent);
        }

        private void FillTags(string Name,byte[] content, long offset, int size)
        {
            switch (Name)
            {
                case "uuid":
                    byte[] uuidVal = new byte[UUIDLen];
                    Array.Copy(content, offset, uuidVal, 0, UUIDLen);
                    if (uuidVal.SequenceEqual(Hex2Binary( "85c0b687820f11e08111f4ce462b6a48")) || uuidVal.SequenceEqual(Hex2Binary("5766b829bb6a47c5bcfb8b9f2260d06d")) || uuidVal.SequenceEqual(Hex2Binary("210f1687914911e4811100242131fce4")))
                        lstCRXIFD.Add(new CRXIFD(content, (uint)(offset + no + UUIDLen)));
                    if(uuidVal.SequenceEqual(Hex2Binary("eaf42b5e1c984b88b9fbb7dc406e4d16")))
                        lstCRXIFD.Add(new CRXIFD(content, (uint)(offset + no + UUIDLen+8)));
                    break;
                default:
                    break;
                    
            }

        }

        public string Name { get; set; }



        static byte[] Hex2Binary(string hexvalue)
        {
            List<byte> binaryval = new List<byte>();
            for (int i = 0; i < hexvalue.Length; i+=2)
            {
                string byteString = hexvalue.Substring(i, 2);
                byte b = Convert.ToByte(byteString, 16);
                binaryval.Add(b);
            }
            return binaryval.ToArray();
        }
        public uint NextIFDOffset { get; set; }

        public int Size { get { return size; } }
        public CRXIFD[] SubIFD { get { return lstCRXIFD.ToArray(); } }

    }
}
