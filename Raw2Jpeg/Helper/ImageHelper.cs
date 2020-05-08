using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Raw2Jpeg.Helper
{
    public static class ImageHelper
    {
        public static byte[] ConvertTo8Bits(int width, int height, ref byte[] pixels16)
        {
            byte[] bReturn = default(byte[]);
            using (Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, width, height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

                unsafe
                {
                    int pixelSize = 3;
                    int i, j, j1, i1;
                    byte b;
                    ushort sVal;
                    double lPixval;

                    for (i = 0; i < bmd.Height; ++i)
                    {
                        byte* row = (byte*)bmd.Scan0 + (i * bmd.Stride);
                        i1 = i * bmd.Height;

                        for (j = 0; j < bmd.Width; ++j)
                        {
                            sVal = (ushort)(pixels16[i1 + j]);
                            lPixval = (sVal / 255.0);
                            if (lPixval > 255) lPixval = 255;
                            if (lPixval < 0) lPixval = 0;
                            b = (byte)(lPixval);
                            j1 = j * pixelSize;
                            row[j1] = b;
                            row[j1 + 1] = b;
                            row[j1 + 2] = b;
                        }
                    }
                }
                bmp.UnlockBits(bmd);
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    ms.Position = 0;
                    bReturn = ms.ToArray();
                }
            }
            return bReturn;
        }

        public static byte[] byteArrayToBMP(int width, int height, ref byte[] pixels)
        {
            byte[] bReturn=default(byte[]);
            using (Bitmap bmp = new Bitmap(width, height))
            {
                BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                IntPtr ptrFirstPixel = bmd.Scan0;
                Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
                bmp.UnlockBits(bmd);
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    ms.Position = 0;
                    bReturn = ms.ToArray();
                }
            }
            return bReturn;
        }


        public static byte[] BayerDemosaic24(int width, int height, ref byte[] pixels24)
        {
            int widthPixel = width * 3;
            byte[] bReturn = new byte[widthPixel * height];


            YCbCr[,] yCbCrs = new YCbCr[height, width];
            RGB[,] rGBs = new RGB[height, width];



            for (int i = 0; i < height; ++i)
            {
                int currentLine = i * width * 3;

                for (int j = 0; j < widthPixel; j += 3)
                {
                    rGBs[i,j/3] = new RGB(pixels24[currentLine + j], pixels24[currentLine + j + 1], pixels24[currentLine + j + 2]);
                    yCbCrs[i, j/3] = SpaceColorConverter.RGBToYCbCr(rGBs[i,j/3]);
                }
            }

            bool isBColumn = false;

            bool isBRow = false;
            bool isBPosition, isRPosition, isGPosition;

            for (int i = 0; i < height; ++i)
            {
                //determine blue row
                isBRow = i % 2 == 1;

                for (int j = 0; j < width; j += 3)
                {
                    if (i < 2 || j < 2 || i > height - 3 || j > width - 3)
                        continue;
                    byte r=0, g=0, b=0;

                    //determine blue column
                    isBColumn = (j / 3) % 2 == 1;

                    //determine position
                    if (isBColumn && isBRow)
                    {
                        isBPosition = true;
                        isRPosition=isGPosition = false;
                    }
                    else if(!isBColumn && !isBRow)
                    {
                        isRPosition = true;
                        isBPosition = isGPosition = false;
                    }
                    else
                    {
                        isGPosition = true;
                        isRPosition = isBPosition = false;
                    }

                    //Built 8 case 
                    if (isRPosition)
                    {
                        //G at R position
                        //Get Chroma
                        float cbG = (yCbCrs[i - 1, j].Cb + yCbCrs[i + 1, j].Cb + yCbCrs[i, j - 1].Cb + yCbCrs[i, j + 1].Cb)/4.0f;
                        float crG = (yCbCrs[i - 1, j].Cr + yCbCrs[i + 1, j].Cr + yCbCrs[i, j - 1].Cr + yCbCrs[i, j + 1].Cr) / 4.0f;
                        //Get Luminance
                        float yG = (4 * yCbCrs[i, j].Y + 2 * (yCbCrs[i - 1, j].Y + yCbCrs[i + 1, j].Y + yCbCrs[i, j - 1].Y + yCbCrs[i, j + 1].Y) - (yCbCrs[i - 2, j].Y + yCbCrs[i + 2, j].Y + yCbCrs[i, j - 2].Y + yCbCrs[i, j + 2].Y))/8;
                        g = SpaceColorConverter.YCbCrToRGB(new YCbCr(yG, cbG, crG)).G;

                        //B at red in R row R column
                        //Get Chroma
                        float cbB = (yCbCrs[i - 1, j - 1].Cb + yCbCrs[i + 1, j - 1].Cb + yCbCrs[i - 1, j + 1].Cb + yCbCrs[i + 1, j + 1].Cb)/4.0f;
                        float crB = (yCbCrs[i - 1, j - 1].Cr + yCbCrs[i + 1, j - 1].Cr + yCbCrs[i - 1, j + 1].Cr + yCbCrs[i + 1, j + 1].Cr)/4.0f;
                        //Get Luminance
                        float yB = (6 * yCbCrs[i, j].Y + 2 * (yCbCrs[i - 1, j-1].Y + yCbCrs[i + 1, j-1].Y + yCbCrs[i-1, j + 1].Y + yCbCrs[i+1, j + 1].Y) - 3/2*(yCbCrs[i - 2, j].Y + yCbCrs[i + 2, j].Y + yCbCrs[i, j - 2].Y + yCbCrs[i, j + 2].Y)) / 20;
                        b = SpaceColorConverter.YCbCrToRGB(new YCbCr(yB, cbB, crB)).B;

                        r = SpaceColorConverter.YCbCrToRGB(yCbCrs[i, j]).R;

                    }
                    if (isGPosition)
                    {

                        if (isBRow)
                        {
                            //R at green in B row  R column
                            //Get Chroma
                            float cbR = (yCbCrs[i, j-1].Cb + yCbCrs[i, j+1].Cb)/2.0f ;
                            float crR = (yCbCrs[i, j - 1].Cr + yCbCrs[i, j + 1].Cr) / 2.0f;
                            //Get Luminance
                            float yR = (5 * yCbCrs[i, j].Y + 4 * (yCbCrs[i, j-1].Y+ yCbCrs[i, j + 1].Y)-(yCbCrs[i-1, j - 1].Y+ yCbCrs[i+1, j - 1].Y+ yCbCrs[i-1, j + 1].Y+ yCbCrs[i-1, j + 1].Y+yCbCrs[i, j - 2].Y+ yCbCrs[i, j +2 ].Y)+1/2*(yCbCrs[i-2, j].Y + yCbCrs[i + 2, j].Y) )/8.0f;
                            r = SpaceColorConverter.YCbCrToRGB(new YCbCr(yR, cbR, crR)).R;

                            //B at green in B row R column
                            //Get Chroma
                            float cbB = (yCbCrs[i - 1, j].Cb + yCbCrs[i + 1, j].Cb)/2.0f;
                            float crB = (yCbCrs[i - 1, j].Cr + yCbCrs[i + 1, j].Cr)/2.0f;
                            //Get Luminance
                            float yB = (5 * yCbCrs[i, j].Y + 4 * (yCbCrs[i-1, j].Y + yCbCrs[i+1, j].Y) - (yCbCrs[i - 1, j - 1].Y + yCbCrs[i + 1, j - 1].Y + yCbCrs[i - 1, j + 1].Y + yCbCrs[i - 1, j + 1].Y + yCbCrs[i, j - 2].Y + yCbCrs[i, j + 2].Y) + 1 / 2 * (yCbCrs[i - 2, j].Y + yCbCrs[i + 2, j].Y)) / 8.0f;
                            b = SpaceColorConverter.YCbCrToRGB(new YCbCr(yB, cbB, crB)).B;

                            g = SpaceColorConverter.YCbCrToRGB(yCbCrs[i, j]).G;

                        }
                        else
                        {
                            //R at green in R row  B column
                            //Get Chroma
                            float cbR = (yCbCrs[i - 1, j].Cb + yCbCrs[i + 1, j].Cb) / 2.0f;
                            float crR = (yCbCrs[i - 1, j].Cr + yCbCrs[i + 1, j].Cr) / 2.0f;
                            //Get Luminance
                            float yR = (5 * yCbCrs[i, j].Y + 4 * (yCbCrs[i - 1, j].Y + yCbCrs[i + 1, j].Y) - (yCbCrs[i - 1, j - 1].Y + yCbCrs[i + 1, j - 1].Y + yCbCrs[i - 1, j + 1].Y + yCbCrs[i - 1, j + 1].Y + yCbCrs[i, j - 2].Y + yCbCrs[i, j + 2].Y) + 1 / 2 * (yCbCrs[i - 2, j].Y + yCbCrs[i + 2, j].Y)) / 8.0f;
                            r = SpaceColorConverter.YCbCrToRGB(new YCbCr(yR, cbR, crR)).R;

                            //B at green in R row B column
                            //Get Chroma
                            float cbB = (yCbCrs[i, j - 1].Cb + yCbCrs[i, j + 1].Cb) / 2.0f;
                            float crB = (yCbCrs[i, j - 1].Cr + yCbCrs[i, j + 1].Cr) / 2.0f;
                            //Get Luminance
                            float yB = (5 * yCbCrs[i, j].Y + 4 * (yCbCrs[i, j - 1].Y + yCbCrs[i, j + 1].Y) - (yCbCrs[i - 1, j - 1].Y + yCbCrs[i + 1, j - 1].Y + yCbCrs[i - 1, j + 1].Y + yCbCrs[i - 1, j + 1].Y + yCbCrs[i, j - 2].Y + yCbCrs[i, j + 2].Y) + 1 / 2 * (yCbCrs[i - 2, j].Y + yCbCrs[i + 2, j].Y)) / 8.0f;
                            b = SpaceColorConverter.YCbCrToRGB(new YCbCr(yB, cbB, crB)).B;

                            g = SpaceColorConverter.YCbCrToRGB(yCbCrs[i, j]).G;
                        }

                    }
                    if (isBPosition)
                    {
                        //G at B position
                        float cbG = (yCbCrs[i - 1, j].Cb + yCbCrs[i + 1, j].Cb + yCbCrs[i, j - 1].Cb + yCbCrs[i, j + 1].Cb)/4.0f;
                        float crG = (yCbCrs[i - 1, j].Cr + yCbCrs[i + 1, j].Cr + yCbCrs[i, j - 1].Cr + yCbCrs[i, j + 1].Cr)/4.0f;
                        //Get Luminance
                        float yG = (4 * yCbCrs[i, j].Y + 2 * (yCbCrs[i - 1, j].Y + yCbCrs[i + 1, j].Y + yCbCrs[i, j - 1].Y + yCbCrs[i, j + 1].Y) - (yCbCrs[i - 2, j].Y + yCbCrs[i + 2, j].Y + yCbCrs[i, j - 2].Y + yCbCrs[i, j + 2].Y)) / 8;
                        g = SpaceColorConverter.YCbCrToRGB(new YCbCr(yG, cbG, crG)).G;

                        //R at blue in B row  B column
                        //Get Chroma
                        float cbR = (yCbCrs[i - 1, j - 1].Cb + yCbCrs[i + 1, j - 1].Cb + yCbCrs[i - 1, j + 1].Cb + yCbCrs[i + 1, j + 1].Cb)/4.0f;
                        float crR = (yCbCrs[i - 1, j - 1].Cr + yCbCrs[i + 1, j - 1].Cr + yCbCrs[i - 1, j + 1].Cr + yCbCrs[i + 1, j + 1].Cr)/4.0f;
                        //Get Luminance
                        float yR = (6 * yCbCrs[i, j].Y + 2 * (yCbCrs[i - 1, j - 1].Y + yCbCrs[i + 1, j - 1].Y + yCbCrs[i - 1, j + 1].Y + yCbCrs[i + 1, j + 1].Y) - 3 / 2 * (yCbCrs[i - 2, j].Y + yCbCrs[i + 2, j].Y + yCbCrs[i, j - 2].Y + yCbCrs[i, j + 2].Y)) / 20;
                        r = SpaceColorConverter.YCbCrToRGB(new YCbCr(yR, cbR, crR)).R;

                        b = SpaceColorConverter.YCbCrToRGB(yCbCrs[i, j]).B;
                        rGBs[i, j] = new RGB(r, g, b);

                    }
                    rGBs[i, j] = new RGB(r, g, b);

                }
            }
            for (int i = 0; i < height; ++i)
            {
                int currentLine = i * width * 3;

                for (int j = 0; j < widthPixel; j += 3)
                {
                    bReturn[currentLine + j] = rGBs[i, j/3].R;
                    bReturn[currentLine + j+1] = rGBs[i, j/3].G;
                    bReturn[currentLine + j+2] = rGBs[i, j/3].B;
                }
            }
            return bReturn;
        }


        public static byte[] ConvertFrom16bits(int width, int height, ref byte[] pixels16)
        {

            ushort red_mask = 0xF800;
            ushort green_mask = 0x7E0;
            ushort blue_mask = 0x1F;

            int byteCount = width * 3 * height;
            byte[] pixels = new byte[byteCount];
            int heightInPixels = height;

            for (int i = 0; i < heightInPixels; ++i)
            {
                int currentLine = i * width * 3;

                for (int j = 0; j < width; j++)
                {
                    var sVal = BitConverter.ToUInt16(pixels16, i * width * 2 + j * 2);
                    var b5 = (sVal & blue_mask);
                    var g6 = ((sVal & green_mask) >> 5) * 255 / 63;
                    var r5 = ((sVal & red_mask) >> 11) * 255 / 31;

                    var r8 = (r5 * 527 + 23) >> 6;
                    var g8 = (g6 * 259 + 33) >> 6;
                    var b8 = (b5 * 527 + 23) >> 6;


                    pixels[currentLine + j * 3] = (byte)r8;
                    pixels[currentLine + j * 3 + 1] = (byte)g8;
                    pixels[currentLine + j * 3 + 2] = (byte)b8;
                }
            }
            //Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            //bmp.UnlockBits(bmd);
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    bmp.Save(ms, ImageFormat.Jpeg);
            //    ms.Position = 0;
            //    bReturn = ms.ToArray();
            //}

            return pixels;
        }

        public static byte[] ConvertFromUncompressed(int width, int height, ref byte[] pixels16)
        {
            byte[] bReturn = default(byte[]);
            var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            byte bpp = 3;
            var BoundsRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            bmp.PixelFormat);
            for (int y = 0; y < height; y++)
                Marshal.Copy(pixels16, y * width * bpp, bmpData.Scan0 + bmpData.Stride * y, width * bpp);
            bmp.UnlockBits(bmpData);
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Jpeg);
                ms.Position = 0;
                bReturn = ms.ToArray();
            }
            return bReturn;
        }




        public static void AutoOrientation(int orientation, ref byte[] input, out byte[] output)
        {
            output = default(byte[]);
            using (MemoryStream ms = new MemoryStream(input))
            {
                using (Bitmap bmp = (Bitmap)Bitmap.FromStream(ms))
                {
                    switch (orientation)
                    {
                        case 1:
                            break;
                        case 2:
                            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            break;
                        case 3:
                            bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 4:
                            bmp.RotateFlip(RotateFlipType.Rotate180FlipY);
                            break;
                        case 5:
                            bmp.RotateFlip(RotateFlipType.Rotate90FlipY);
                            break;
                        case 6:
                            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 7:
                            bmp.RotateFlip(RotateFlipType.Rotate270FlipY);
                            break;
                        case 8:
                            bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;

                    }
                    using (MemoryStream msOutput = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Jpeg);
                        msOutput.Position = 0;
                        output = msOutput.ToArray();
                    }
                }
            }
        }


        private static Size ResizeKeepAspect(Size src, int maxWidth, int maxHeight, bool ratio = false)
        {
            double dratio = Math.Max(src.Width, src.Height) / Math.Min(src.Width, src.Height);


            maxWidth = ratio ? (maxWidth == 0 ? (int)(maxHeight * dratio) : maxWidth) : Math.Min(maxWidth, src.Width);
            maxHeight = ratio ? (maxHeight == 0 ? (int)(maxWidth * dratio) : maxHeight) : Math.Min(maxHeight, src.Height);

            decimal rnd = Math.Min(maxWidth / (decimal)src.Width, maxHeight / (decimal)src.Height);
            return new Size((int)Math.Round(src.Width * rnd), (int)Math.Round(src.Height * rnd));
        }
        public static Bitmap ResizeBitmap(Stream stream, int width, int height, bool ratio = false)
        {
            return ResizeBitmap((Bitmap)Bitmap.FromStream(stream), width, height, ratio);
        }

        public static Stream ResizeBitmapAsStream(Stream stream, int width, int height, bool ratio = false)
        {
            using (Bitmap bmp = (Bitmap)Bitmap.FromStream(stream))
            {
                using (Bitmap bmpResize = ResizeBitmap((Bitmap)Bitmap.FromStream(stream), width, height, ratio))
                {
                    MemoryStream ms = new MemoryStream();
                    bmpResize.Save(ms, bmp.RawFormat);
                    return ms;
                }
            }
        }
        public static Bitmap ResizeBitmap(Bitmap bmp, int width, int height, bool ratio = false)
        {
            Size NewSize;
            if (ratio)
                NewSize = ResizeKeepAspect(new Size(bmp.Width, bmp.Height), width, height, true);
            else
                NewSize = new Size(width, height);
            return new Bitmap(bmp, NewSize);
        }

        public static Bitmap CropBitmap(Bitmap bmp, int width, int height, int x, int y)
        {
            Bitmap result = new Bitmap(width, height);
            Rectangle cropRect = new Rectangle(x, y, width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, new Rectangle(0, 0, result.Width, result.Height), cropRect, GraphicsUnit.Pixel);
            }

            return result;
        }

        public static string ImageToBase64(Bitmap bmp)
        {
            using (MemoryStream m = new MemoryStream())
            {
                bmp.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }

        }



    }
}
