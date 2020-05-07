
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
using System.Text;

namespace Raw2Jpeg.Helper
{
    internal static class Bayer16
    {
        private static void CLIP16(int input, int output, int bits)
        {
            input = input < 0 ? 0 : input;
            input = input > ((1 << bits) - 1) ? ((1 << bits) - 1) : input;
            output = input;
        }
        private static void ClearBorders_uint16(ushort[] rgb, int sx, int sy, int w)
        {
            int i;
            int j;

            // black edges:
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




        internal static dc1394error_t dc1394_bayer_NearestNeighbor_uint16(ushort bayer, ushort rgb, uint sx, uint sy, dc1394color_filter_t tile, uint bits)
        {
            throw new NotImplementedException();
        }

        internal static dc1394error_t dc1394_bayer_Simple_uint16(ushort bayer, ushort rgb, uint sx, uint sy, dc1394color_filter_t tile, uint bits)
        {
            throw new NotImplementedException();
        }

        internal static dc1394error_t dc1394_bayer_Bilinear_uint16(ushort bayer, ushort rgb, uint sx, uint sy, dc1394color_filter_t tile, uint bits)
        {
            throw new NotImplementedException();
        }

        internal static dc1394error_t dc1394_bayer_HQLinear_uint16(ushort bayer, ushort rgb, uint sx, uint sy, dc1394color_filter_t tile, uint bits)
        {
            throw new NotImplementedException();
        }

        internal static dc1394error_t dc1394_bayer_Downsample_uint16(ushort bayer, ushort rgb, uint sx, uint sy, dc1394color_filter_t tile, uint bits)
        {
            throw new NotImplementedException();
        }

        internal static dc1394error_t dc1394_bayer_EdgeSense_uint16(ushort bayer, ushort rgb, uint sx, uint sy, dc1394color_filter_t tile, uint bits)
        {
            throw new NotImplementedException();
        }

        internal static dc1394error_t dc1394_bayer_VNG_uint16(ushort bayer, ushort rgb, uint sx, uint sy, dc1394color_filter_t tile, uint bits)
        {
            throw new NotImplementedException();
        }

        internal static dc1394error_t dc1394_bayer_AHD_uint16(ushort bayer, ushort rgb, uint sx, uint sy, dc1394color_filter_t tile, uint bits)
        {
            throw new NotImplementedException();
        }
    }
}
