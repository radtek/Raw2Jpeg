using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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


        public static byte[] ConvertFrom16bits(int width, int height, ref byte[] pixels16)
        {
            byte[] bReturn = default(byte[]);
            using (Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                BitmapData bmd = bmp.LockBits(new Rectangle(0, 0, width, height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

                ushort red_mask = 0xF800;
                ushort green_mask = 0x7E0;
                ushort blue_mask = 0x1F;

                int byteCount = bmd.Stride * bmp.Height;
                byte[] pixels = new byte[byteCount];
                IntPtr ptrFirstPixel = bmd.Scan0;
                Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
                int heightInPixels = bmp.Height;

                for (int i = 0; i < heightInPixels; ++i)
                {
                    int currentLine = i * bmd.Stride;

                    for (int j = 0; j < bmp.Width; j++)
                    {
                        var sVal = BitConverter.ToUInt16(pixels16, i * bmp.Width * 2 + j * 2);
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
