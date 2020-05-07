/************************************************************************
 *     Color conversion functions for cameras that can                  *
 * output raw-Bayer pattern images, such as some Basler and             *
 * Point Grey camera. The code come from the github repo                *
 * https://raw.githubusercontent.com/jdthomas/bayer2rgb/master/bayer.c  *
 * converted to c# standard library                                     *
 *                                                                      *
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Raw2Jpeg.Helper
{
    internal static class Bayer8
    {
        private const dc1394color_filter_t DC1394_COLOR_FILTER_MIN = dc1394color_filter_t.DC1394_COLOR_FILTER_RGGB;
        private const dc1394color_filter_t DC1394_COLOR_FILTER_MAX = dc1394color_filter_t.DC1394_COLOR_FILTER_BGGR;
        private static void CLIP(int input, int output)
        {
            input = input < 0 ? 0 : input;
            input = input > 255 ? 255 : input;
            output = input;
        }

        private static void ClearBorders(byte[] rgb, int sx, int sy, int w)
        {
            int i;
            int j;
            // black edges are added with a width w:
            i = 3 * sx * w - 1;
            j = 3 * sx * sy - 1;
            while (i >= 0)
            {
                rgb[i--] = 0;
                rgb[j--] = 0;
            }

            int low = sx * (w - 1) * 3 - 1 + w * 3;
            i = low + sx * (sy - w * 2 + 1) * 3;
            while (i > low)
            {
                j = 6 * w;
                while (j > 0)
                {
                    rgb[i--] = 0;
                    j--;
                }
                i -= (sx - 2 * w) * 3;
            }
        }


        internal static dc1394error_t dc1394_bayer_Simple(byte[] bayer,out byte[] rgb, uint sx, uint sy, dc1394color_filter_t tile)
        {
			uint bayerStep = sx;
			uint rgbStep = 3 * sx;
			uint width = sx;
			uint height = sy;
			int blue = tile == dc1394color_filter_t.DC1394_COLOR_FILTER_BGGR || tile == dc1394color_filter_t.DC1394_COLOR_FILTER_GBRG ? -1 : 1;
			bool start_with_green = tile == dc1394color_filter_t.DC1394_COLOR_FILTER_GBRG || tile == dc1394color_filter_t.DC1394_COLOR_FILTER_GRBG;
			uint RGBPos=0,BayerPos=0;
			uint imax;
			uint iinc;
			uint i;

			imax = sx * sy * 3;
			rgb = new byte[imax];


			if ((tile > DC1394_COLOR_FILTER_MAX) || (tile < DC1394_COLOR_FILTER_MIN))
			{
				return dc1394error_t.DC1394_INVALID_COLOR_FILTER;
			}

			/* add black border */

			for (i = sx * (sy - 1) * 3; i < imax; i++)
			{
				rgb[i] = 0;
			}
			iinc = (sx - 1) * 3;
			for (i = (sx - 1) * 3; i < imax; i += iinc)
			{
				rgb[i++] = 0;
				rgb[i++] = 0;
				rgb[i++] = 0;
			}

			RGBPos += 1;
			width -= 1;
			height -= 1;

			for (; height-->0; BayerPos += bayerStep, RGBPos += rgbStep)
			{
				//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent to pointers to value types:
				//ORIGINAL LINE: const byte *bayerEnd = bayer + width;
				byte bayerEnd = bayer[BayerPos + width];

				if (start_with_green)
				{
					rgb[RGBPos-blue] = bayer[bayerStep+1];
					rgb[RGBPos] = (byte)((bayer[bayerStep] + bayer[bayerStep + 1] + 1) >> 1);
					rgb[RGBPos+blue] = bayer[bayerStep];
					BayerPos++;
					RGBPos += 3;
				}

				if (blue > 0)
				{
					for (; BayerPos <= bayerEnd - 2; BayerPos += 2, RGBPos += 6)
					{
						rgb[RGBPos-1] = bayer[BayerPos];
						rgb[RGBPos] = (byte)((bayer[BayerPos+1] + bayer[BayerPos+bayerStep] + 1) >> 1);
						rgb[RGBPos+1] = bayer[BayerPos+bayerStep + 1];

						rgb[RGBPos+2] = bayer[BayerPos+2];
						rgb[RGBPos+3] = (byte)((bayer[BayerPos+1] + bayer[BayerPos+bayerStep + 2] + 1) >> 1);
						rgb[RGBPos+4] = bayer[BayerPos+bayerStep + 1];
					}
				}
				else
				{
					for (; BayerPos <= bayerEnd - 2; BayerPos += 2, RGBPos += 6)
					{
						rgb[RGBPos+ 1] = bayer[BayerPos];
						rgb[RGBPos] = (byte)((bayer[BayerPos+1] + bayer[BayerPos+bayerStep] + 1) >> 1);
						rgb[RGBPos -1] = bayer[BayerPos+bayerStep + 1];

						rgb[RGBPos + 4] = bayer[BayerPos+2];
						rgb[RGBPos + 3] =(byte)((bayer[BayerPos+1] + bayer[BayerPos+bayerStep + 2] + 1) >> 1);
						rgb[RGBPos + 2] = bayer[BayerPos+bayerStep + 1];
					}
				}

				if (BayerPos < bayerEnd)
				{
					rgb[RGBPos - blue] = bayer[BayerPos];
					rgb[RGBPos] =(byte)( (bayer[BayerPos+1] + bayer[BayerPos+bayerStep] + 1) >> 1);
					rgb[RGBPos+blue] = bayer[bayerStep + 1];
					BayerPos++;
					RGBPos += 3;
				}

				BayerPos -= width;
				RGBPos -= width * 3;

				blue = -blue;
				start_with_green = !start_with_green;
			}

			return dc1394error_t.DC1394_SUCCESS;


		}

		internal static dc1394error_t dc1394_bayer_AHD(byte[] bayer, out byte[] rgb, uint sx, uint sy, dc1394color_filter_t tile)
		{
			throw new NotImplementedException();
		}

		internal static dc1394error_t dc1394_bayer_VNG(byte[] bayer, out byte[] rgb, uint sx, uint sy, dc1394color_filter_t tile)
		{
			throw new NotImplementedException();
		}

		internal static dc1394error_t dc1394_bayer_EdgeSense(byte[] bayer, out byte[] rgb, uint sx, uint sy, dc1394color_filter_t tile)
		{
			throw new NotImplementedException();
		}

		internal static dc1394error_t dc1394_bayer_Downsample(byte[] bayer, out byte[] rgb, uint sx, uint sy, dc1394color_filter_t tile)
		{
			throw new NotImplementedException();
		}

		internal static dc1394error_t dc1394_bayer_HQLinear(byte[] bayer, out byte[] rgb, uint sx, uint sy, dc1394color_filter_t tile)
		{
			throw new NotImplementedException();
		}

		internal static dc1394error_t dc1394_bayer_Bilinear(byte[] bayer, out byte[] rgb, uint sx, uint sy, dc1394color_filter_t tile)
		{
			throw new NotImplementedException();
		}

		internal static dc1394error_t dc1394_bayer_NearestNeighbor(byte[] bayer, out byte[] rgb, uint sx, uint sy, dc1394color_filter_t tile)
		{
			throw new NotImplementedException();
		}
	}
}
